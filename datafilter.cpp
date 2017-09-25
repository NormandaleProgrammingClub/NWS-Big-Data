//Data Filter NWS Data Project
//Normandale Computer Programming Club
//Written By: Ben Olson

#include <iostream>
#include <fstream>
#include <string>

	using namespace std;
	
int main()
{
	string file;
	vector <vector <string>> line;
	char delimiter;
	int columns = 0;
	
	
	cout << "Please input the complete file directory" << endl;
	cin >> file;
	cout << "Please input the file delimiter" << endl;
	cin >> delimiter;
	cout << "Please input the columns, with no spaces, that you would like:" << endl;
	cin >> columns;
	cout << file << " " << delimiter << " " << columns << endl;
	
	
	fstream data(file);
	if (!data) cerr << "Can't open input file!" << endl;

	while (getline(data,line))




	return 0;
}