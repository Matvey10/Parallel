// 32.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <iostream>
using namespace std;
#define MAX_N 15
int main()
{
	int a[MAX_N][MAX_N];
	for (int i = 0; i < MAX_N; ++i)
	{
		memset(a[i], 0, MAX_N * sizeof(a[i][0]));
		a[i][0] = a[i][i] = 1;
	}
	for (int i = 1; i < MAX_N; ++i)
		for (int j = 1; j < i; ++j)
		{
			a[i][j] = a[i - 1][j] + a[i - 1][j - 1];
			if (i == 6)
				printf("%d", a[i][j]);
		}
	int p;
	scanf_s("%d", &p);
	return 0;
}

