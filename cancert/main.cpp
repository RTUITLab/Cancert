#include <opencv2/opencv.hpp>

using namespace cv;

Mat origin;

struct ThresholdState
{
	int adaptiveThreshold = 100;
	int blockSize = 43;
	int C = 150;
} thresholdState;

double getMean(Mat channel)
{
	int histSize = 256;    // bin size
	float range[] = { 0, 255 };
	const float *ranges[] = { range };

	MatND hist;
	calcHist(&channel, 1, 0, Mat(), hist, 1, &histSize, ranges, true, false);

	int hist_w = 255; int hist_h = 255;
	int bin_w = cvRound((double)hist_w / histSize);

	Mat histImage(hist_h, hist_w, CV_8UC1, Scalar(0, 0, 0));
	normalize(hist, hist, 0, histImage.rows, NORM_MINMAX);

	for (int i = 1; i < histSize; i++) {
		line(histImage, Point(bin_w*(i - 1), hist_h - cvRound(hist.at<float>(i - 1))),
			Point(bin_w*(i), hist_h - cvRound(hist.at<float>(i))),
			Scalar(255, 0, 0), 2, 8, 0);
	}

	imshow("Hist", histImage);

	auto model = ml::EM::create();
	model->setClustersNumber(3);
	model->trainEM(hist);

	auto means = model->getMeans();
	sort(means, means, CV_SORT_EVERY_COLUMN + CV_SORT_ASCENDING);

	std::cout << means << std::endl;

	return means.at<double>(1);
}

void processImage()
{
	Mat originalMat = origin.clone();

	threshold(originalMat, originalMat, 30, 255, CV_THRESH_TOZERO);
	normalize(originalMat, originalMat, 0, 255, NORM_MINMAX);

	fastNlMeansDenoising(originalMat, originalMat, 3, 7, 21);
	imshow("Original", originalMat);

	Mat thresholdedMat = originalMat.clone();
	threshold(originalMat, thresholdedMat, 30, 255, CV_THRESH_BINARY);

	std::vector<std::vector<Point>> contours;
	Mat contourOutput = thresholdedMat.clone();
	findContours(contourOutput, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);

	Rect minimalRect = boundingRect(contours[0]);

	Mat craniumMask = Mat::zeros(originalMat.size(), CV_8UC1);
	for (size_t idx = 0; idx < contours.size(); idx++) {
		drawContours(craniumMask, contours, idx, Scalar(255), 0.0004f * minimalRect.size().area());
	}

	subtract(originalMat, craniumMask, originalMat);

	Mat adaptiveThresholdedMat;
	adaptiveThreshold(originalMat, adaptiveThresholdedMat, 
		thresholdState.adaptiveThreshold, ADAPTIVE_THRESH_GAUSSIAN_C, THRESH_BINARY,
		thresholdState.blockSize, thresholdState.C - 100);
	subtract(originalMat, adaptiveThresholdedMat, originalMat);

	Mat structuringElement = getStructuringElement(MORPH_ELLIPSE, Size(40, 40));
	morphologyEx(originalMat, originalMat, MORPH_CLOSE, structuringElement);

	Mat resultMat = origin.clone();
	cvtColor(resultMat, resultMat, CV_GRAY2RGB);
	Scalar colors[3];
	contours.clear();
	findContours(originalMat, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);
	for (size_t idx = 0; idx < contours.size(); idx++) {
		drawContours(resultMat, contours, idx, Scalar(0, 255, 0), 1);
	}

	rectangle(resultMat, minimalRect, Scalar(255, 255, 255), 1, 8, 0);

	imshow("Final", resultMat);
}

int main(int argc, char* argv[])
{
	origin = imread(argv[1], IMREAD_GRAYSCALE); //input image

	thresholdState.adaptiveThreshold = getMean(origin);

	namedWindow("Final", 1);

	createTrackbar("Threshold", "Final", &thresholdState.adaptiveThreshold, 255, [](int, void*) {
		processImage();
	});

	createTrackbar("Block size", "Final", &thresholdState.blockSize, 255, [](int, void*) {
		thresholdState.blockSize += (thresholdState.blockSize + 1) % 2;
		if (thresholdState.blockSize < 3) {
			thresholdState.blockSize = 3;
		}
		processImage();
	});

	createTrackbar("Threshold constant", "Final", &thresholdState.C, 200, [](int, void*) {
		processImage();
	});

	processImage();

	waitKey();
}