import pygame, math
pygame.init()

screen_w = 400
screen_h = 400
screen_wh_min = 400
screen_wh_max = 800
center_x = screen_w / 2
center_y = screen_h / 2
# compass radius
radius = 150
# dashes in compass
dashes = 8

screen = None
pygame.display.set_caption("Buoy Data Viewer")

def window_resize(new_size):
	global screen, screen_w, screen_h, center_x, center_y, radius

	screen_w, screen_h = new_size

	if screen_w < screen_wh_min:
		screen_w = screen_wh_min
	if screen_w > screen_wh_max:
		screen_w = screen_wh_max

	if screen_h < screen_wh_min:
		screen_h = screen_wh_min
	if screen_h > screen_wh_max:
		screen_h = screen_wh_max

	screen = pygame.display.set_mode((screen_w, screen_h), pygame.RESIZABLE)
	center_x = screen_w / 2
	center_y = screen_h / 2
	radius = min(screen_w, screen_h)/2 - 50


def draw_compass():
	for i in range(32):
		angle = 11.25 * i
		angle = math.radians(angle)

		cos = math.cos(angle)
		sin = math.sin(angle)

		# Determines length of dash
		if i in (0, 4, 8, 12, 16, 20, 24, 28):
			inner_radius = radius - (min(screen_w, screen_h) / 20)
		elif i in (2, 6, 10, 14, 18, 22, 26):
			inner_radius = radius - (min(screen_w, screen_h) / 25)
		else:
			inner_radius = radius - (min(screen_w, screen_h) / 80)

		# Handles each quatrent seperately
		if i <= 4:
			outer_x = cos * radius + center_x
			outer_y = center_y - sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_y - sin * inner_radius
		elif i > 4 <= 8:
			outer_x = center_x + cos * radius
			outer_y = center_y - sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_y - sin * inner_radius
		elif i > 8 <= 12:
			outer_x = cos * radius + center_x
			outer_y = center_y + sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_y + sin * inner_radius
		else:
			outer_x = center_y - cos * radius
			outer_y = center_x + sin * radius
			inner_x = center_y - cos * inner_radius
			inner_y = center_x + sin * inner_radius

		# Different sized dashed
		if i in (0, 8, 16, 24):
			pygame.draw.line(screen, (191, 0, 0), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 133)) # default width = 3 pixels
		elif i in (4, 12, 20, 28):
			pygame.draw.line(screen, (191, 0, 0), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 200)) # default width = 2 pixels
		else:
			pygame.draw.line(screen, (255, 255, 255), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 400)) # default width = 1 pixels

def draw_windspeed(wind_speed):
	font_big = pygame.font.Font('Roboto-Light.ttf', 50)
	font_small = pygame.font.Font('Roboto-Light.ttf', 15)
	speed_text = font_big.render(str(wind_speed), True, (255, 255, 255)).convert_alpha()
	unit_text = font_small.render('mph', True, (191, 191, 191)).convert_alpha()

	speed_text_rect = speed_text.get_rect()
	unit_text_rect = unit_text.get_rect()

	speed_text_rect.center = (center_x, center_y)
	unit_text_rect.center = (center_x, center_y+30)

	screen.blit(speed_text, speed_text_rect)
	screen.blit(unit_text, unit_text_rect)


def draw_arrow(d):
	height = 60
	# convert buoy directions to pygame directions
	d = math.radians(90 - d)
	outer_rad = 110
	inner_rad = outer_rad - height

	# The outer tip of the triangle
	point1 = [int(center_x + math.cos(d) * outer_rad), int(center_y - math.sin(d) * outer_rad)]

	slope_num = point1[1] - center_y
	slope_den = point1[0] - center_x
	perp_slope_num = int(slope_den)
	perp_slope_den = int(slope_num)
	# The point [height] away from point1 on the line between point1 and center
	inner_point = [int(center_x + math.cos(d) * inner_rad), int(center_y - math.sin(d) * inner_rad)]
	# The two base points of the triangle, each equidistant from inner_point, forming a perpendicular to the line between point1 and center
	point2 = [inner_point[0] + perp_slope_den/10, inner_point[1] - perp_slope_num/10]
	point3 = [inner_point[0] - perp_slope_den/10, inner_point[1] + perp_slope_num/10]


	pygame.draw.polygon(screen, (0, 30, 140), (point1, point2, point3))


window_resize((screen_w, screen_h))

while True:
	for event in pygame.event.get():
		if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
			pygame.quit()
			quit()

		elif event.type == pygame.VIDEORESIZE:
			window_resize(event.dict['size'])

	screen.fill((0, 0, 0))

	draw_compass()
	draw_windspeed(50)
	draw_arrow(337.5)

	pygame.display.update()
