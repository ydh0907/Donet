#include <iostream>
#include <thread>
#include <mutex>

using namespace std;

mutex sumlocker;

void add(int* val, int min, int max) {
	int sum = 0;
	for (int i = min; i <= max; i++) {
		sum += i;
	}
	sumlocker.lock();
	*val += sum;
	sumlocker.unlock();
}

int main()
{
	int num = 0;
	auto now = chrono::steady_clock::now();
	thread t1(add, &num, 1, 33333333);
	thread t2(add, &num, 33333334, 66666666);
	thread t3(add, &num, 66666667, 100000000);
	t1.join();
	t2.join();
	t3.join();
	auto end = chrono::steady_clock::now() - now;

	cout << "multi  : " << end.count() << "ns" << endl;
	cout << num << endl;

	num = 0;
	now = chrono::steady_clock::now();
	for (int i = 1; i <= 100000000; i++)
		num += i;
	end = chrono::steady_clock::now() - now;

	cout << "single : " << end.count() << "ns" << endl;
	cout << num << endl;
}