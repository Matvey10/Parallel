// лр-1-1.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include "Array.cpp"
#include "List.cpp"
#include "sort.cpp"
#include <iostream>
#include <ctime>
using namespace std;
int main()
{
	setlocale(LC_ALL, "rus");
	cout << "hhhhhhh";
	Sequence<int>* sequences[3];//создали массив указателей на послед-сти
	sequences[0] = (Sequence<int>*)new ArraySequence<int>;
	sequences[1] = (Sequence<int>*)new ListSequence<int>;
	for (int i = 0; i < 2; i++)
	{
		auto seq = sequences[i];
		for (int i = 0; i < 10; i++)
		{
			seq->Append(1 + rand() % 10);
		}
		seq = bubbleSort(seq);

	}
	system("pause");
	return 0;
}


