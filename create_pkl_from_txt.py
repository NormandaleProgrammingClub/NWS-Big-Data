# creates a dataframe object with weather data and saves it as pkl file

import pandas

buoy_txt_file = open('2005/41008h2005.txt', 'r')
buoy_data_lines = buoy_txt_file.readlines()
buoy_data_lines.pop(0)
buoy_data = []

for line in buoy_data_lines:
	line_nums = []
	num = ''
	for index in range(len(line)):
		if line[index].isdigit() or line[index] == '.':
			num += line[index]

		elif (line[index] == ' ' and line[index+1] != ' ') or index == len(line)-1:
			if '.' in num:
				line_nums.append(float(num))
			else:
				line_nums.append(int(num))

			num = ''

	buoy_data.append(line_nums)

col_labels = ['YYYY', 'MM', 'DD', 'hh', 'mm', 'WD', 'WSPD', 'GST', 'WVHT', 'DPD', 'APD',  'MWD',  'BAR', 'ATMP', 'WTMP', 'DEWP', 'VIS', 'TIDE']
df = pandas.DataFrame(buoy_data, columns=col_labels)

df.to_pickle('41008h_2005.pkl')
