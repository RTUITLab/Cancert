#include <opencv2/opencv.hpp>

using namespace cv;

int main(int argc, char* argv[])
{
	Mat originalMat = imread(argv[1], IMREAD_GRAYSCALE); //input image
	imshow("Original", originalMat);

	Ptr<CLAHE> clahe = createCLAHE();
	clahe->setClipLimit(0.5);

	//Mat grayscaledMat;
	//clahe->apply(originalMat, grayscaledMat);

	fastNlMeansDenoising(originalMat, originalMat, 3, 7, 21);

	Mat thresholdedMat = originalMat.clone();
	threshold(originalMat, thresholdedMat, 30, 255, CV_THRESH_BINARY);

	std::vector<std::vector<Point>> contours;
	Mat contourOutput = thresholdedMat.clone();
	findContours(contourOutput, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE);

	threshold(originalMat, originalMat, 0, 200, CV_THRESH_OTSU);

	Mat contourImage = originalMat.clone();
	Scalar colors[3];
	colors[0] = Scalar(255, 0, 0);
	for (size_t idx = 0; idx < contours.size(); idx++) {
		drawContours(contourImage, contours, idx, colors[idx % 3]);
	}

	std::cout << contours.size() << "\n";
	RotatedRect minimalRect = fitEllipse(contours[0]);
	Point2f vertices[4];
	minimalRect.points(vertices);
	for (int i = 0; i < 4; i++) {
		line(contourImage, vertices[i], vertices[(i + 1) % 4], Scalar(255), 2);
	}

	imshow("Final", contourImage);

	waitKey();
}