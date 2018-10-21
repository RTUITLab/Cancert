#include <fstream>

#include <opencv2/opencv.hpp>
#include <json.hpp>

using namespace cv;
using json = nlohmann::json;

static const double CRANIUM_AREA_COEFFICIENT = 0.0004;
static const int ADAPTIVE_THRESHOLD_OFFSET = -10;
static const int ADAPTIVE_THRESHOLD_BLOCKSIZE = 43;
static const int ADAPTIVE_THRESHOLD_C = 50;
static const int MORPH_SIZE = 40;

double getMean(Mat channel)
{
	auto histSize = 256;    // bin size
	float range[] = { 0, 255 };
	const float *ranges[] = { range };

	MatND hist;
	calcHist(&channel, 1, nullptr, Mat(), hist, 1, &histSize, ranges, true, false);

	normalize(hist, hist, 0, 255, NORM_MINMAX);

	auto model = ml::EM::create();
	model->setClustersNumber(3);
	model->trainEM(hist);

	auto means = model->getMeans();
	sort(means, means, CV_SORT_EVERY_COLUMN + CV_SORT_ASCENDING);

	std::cout << means << std::endl;

	return means.at<double>(1);
}

void processImage(Mat origin, Mat& result)
{
	auto originalMat = origin.clone();

	threshold(originalMat, originalMat, 30, 255, CV_THRESH_TOZERO);
	normalize(originalMat, originalMat, 0, 255, NORM_MINMAX);
	
	fastNlMeansDenoising(originalMat, originalMat, 3, 7, 21);

	auto thresholdMat = originalMat.clone();
	threshold(originalMat, thresholdMat, 30, 255, CV_THRESH_BINARY);

	std::vector<std::vector<Point>> contours;
	auto contourOutput = thresholdMat.clone();
	findContours(contourOutput, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);

	auto minimalRect = boundingRect(contours[0]);

	const auto craniumThickness = static_cast<int>(CRANIUM_AREA_COEFFICIENT * minimalRect.size().area());
	Mat craniumMask = Mat::zeros(originalMat.size(), CV_8UC1);
	for (size_t idx = 0; idx < contours.size(); idx++) {
		drawContours(craniumMask, contours, idx, Scalar(255), craniumThickness);
	}

	const Rect leftRect(minimalRect.x, minimalRect.y, minimalRect.width / 2, minimalRect.height);
	const Rect rightRect(minimalRect.x + minimalRect.width / 2, minimalRect.y, minimalRect.width / 2, minimalRect.height);

	Mat leftSubImage;
	const Mat leftImage = Mat::zeros(origin.size(), CV_8U);
	Mat(originalMat, leftRect).copyTo(leftSubImage);
	flip(leftSubImage, leftImage(rightRect), 1);

	Mat rightSubImage;
	const Mat rightImage = Mat::zeros(origin.size(), CV_8U);
	Mat(originalMat, rightRect).copyTo(rightSubImage);
	flip(rightSubImage, rightImage(leftRect), 1);

	auto mirroredMat = originalMat.clone();
	subtract(mirroredMat, leftImage, mirroredMat);
	subtract(mirroredMat, rightImage, mirroredMat);

	Mat reflected;
	subtract(originalMat, mirroredMat, reflected);

	auto subtractionMat = originalMat.clone();
	subtractionMat.setTo(0, originalMat > reflected);
	subtract(originalMat, subtractionMat, originalMat);

	subtract(originalMat, craniumMask, originalMat);

	Mat adaptiveThresholdMat;
	adaptiveThreshold(originalMat, adaptiveThresholdMat,
		getMean(originalMat) * 255 + ADAPTIVE_THRESHOLD_OFFSET, 
		ADAPTIVE_THRESH_GAUSSIAN_C, THRESH_BINARY, ADAPTIVE_THRESHOLD_BLOCKSIZE, 
		ADAPTIVE_THRESHOLD_C);

	subtract(originalMat, adaptiveThresholdMat, originalMat);

	const auto structuringElement = getStructuringElement(MORPH_ELLIPSE, Size(MORPH_SIZE, MORPH_SIZE));
	morphologyEx(originalMat, originalMat, MORPH_CLOSE, structuringElement);

	const auto minimalRectCenter = minimalRect.x + minimalRect.width / 2;

	Mat(originalMat, Rect(minimalRectCenter - craniumThickness / 2, 
		minimalRect.y, craniumThickness, craniumThickness * 2)) = Scalar(0);

	Mat(originalMat, Rect(minimalRectCenter - craniumThickness / 2, 
		minimalRect.y + minimalRect.height - craniumThickness * 2, craniumThickness, craniumThickness * 2)) = Scalar(0);

	auto resultMat = origin.clone();
	cvtColor(resultMat, resultMat, CV_GRAY2RGB);
	Scalar colors[3];
	contours.clear();
	findContours(originalMat, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);
	for (size_t idx = 0; idx < contours.size(); idx++) {
		drawContours(resultMat, contours, idx, Scalar(0, 255, 0), 1);
	}

	rectangle(resultMat, minimalRect, Scalar(255, 255, 255), 1, 8, 0);

	result = std::move(resultMat);
	//imshow("Final", resultMat);
}

int main(int argc, char* argv[])
{
	if (argc < 2) {
		return 1;
	}

	std::ifstream inputConfig(argv[1]);
	json config;
	inputConfig >> config;

	auto targetDir = config["TargetDir"].get<std::string>();
	std::cout << targetDir << "\n";

	auto photos = config["Photos"];
	for (const auto& photo : photos) {
		const auto path = photo["Path"].get<std::string>();

		Mat result;
		processImage(imread(path, IMREAD_GRAYSCALE), result);

		const auto dotPos = path.find_last_of('.');
		const auto extension = path.substr(dotPos);
		imwrite(targetDir + path.substr(path.find_last_of('/') + 1, dotPos - 1) + "_result" + extension, result);
	}

	return 0;
}