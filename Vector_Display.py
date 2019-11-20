import pygame, math
from pygame import gfxdraw
pygame.init()

screen_size_slider = 0
screen_sizes = [400, 500, 600, 700, 800]
screen_w = screen_sizes[screen_size_slider]
center_x = screen_w / 2
# compass radius
radius = screen_w - 250
unit = 'mph'
unit_text_rect = None

screen = None
clock = pygame.time.Clock()
pygame.display.set_caption("Buoy Data Viewer")

def window_resize(new_width):
	global screen, screen_w, center_x, radius

	screen_w = new_width

	screen = pygame.display.set_mode((screen_w, screen_w), pygame.RESIZABLE)
	center_x = screen_w / 2
	radius = screen_w / 2 - 50


def draw_compass():
	for i in range(32):
		angle = 11.25 * i
		angle = math.radians(angle)

		cos = math.cos(angle)
		sin = math.sin(angle)

		# Determines length of dash
		if i in (0, 4, 8, 12, 16, 20, 24, 28):
			inner_radius = radius - screen_w / 20
		elif i in (2, 6, 10, 14, 18, 22, 26):
			inner_radius = radius - screen_w / 25
		else:
			inner_radius = radius - screen_w / 80

		# Handles each quatrent seperately
		if i <= 4:
			outer_x = cos * radius + center_x
			outer_y = center_x - sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_x - sin * inner_radius
		elif i > 4 <= 8:
			outer_x = center_x + cos * radius
			outer_y = center_x - sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_x - sin * inner_radius
		elif i > 8 <= 12:
			outer_x = cos * radius + center_x
			outer_y = center_x + sin * radius
			inner_x = cos * inner_radius + center_x
			inner_y = center_x + sin * inner_radius
		else:
			outer_x = center_x - cos * radius
			outer_y = center_x + sin * radius
			inner_x = center_x - cos * inner_radius
			inner_y = center_x + sin * inner_radius

		# Different sized dashed
		if i in (0, 8, 16, 24):
			pygame.draw.line(screen, (191, 0, 0), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 133)) # default width = 3 pixels
		elif i in (4, 12, 20, 28):
			pygame.draw.line(screen, (191, 0, 0), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 200)) # default width = 2 pixels
		else:
			pygame.draw.line(screen, (255, 255, 255), (inner_x, inner_y), (outer_x, outer_y), int(screen_w / 400)) # default width = 1 pixels

def draw_windspeed(wind_speed, unit):
	global unit_text_rect

	if unit == 'kmph':
		wind_speed *= 1.60934

	wind_speed = round(wind_speed)

	font_big = pygame.font.Font('Roboto-Light.ttf', int(screen_w / 8))
	font_small = pygame.font.Font('Roboto-Light.ttf', int(screen_w / 25))
	speed_text = font_big.render(str(wind_speed), True, (255, 255, 255)).convert_alpha()
	unit_text = font_small.render(unit, True, (191, 191, 191)).convert_alpha()

	speed_text_rect = speed_text.get_rect()
	unit_text_rect = unit_text.get_rect()

	speed_text_rect.center = (center_x, center_x)
	unit_text_rect.center = (center_x, center_x + int(screen_w / 13))

	screen.blit(speed_text, speed_text_rect)
	screen.blit(unit_text, unit_text_rect)


def draw_arrow(d):
	height = int(screen_w / 6)
	# convert buoy directions to pygame directions
	d = math.radians(90 - d)
	outer_rad = int(radius * 0.75)
	inner_rad = outer_rad - height

	# The outer tip of the triangle
	point1 = [int(center_x + math.cos(d) * outer_rad), int(center_x - math.sin(d) * outer_rad)]

	slope_num = point1[1] - center_x
	slope_den = point1[0] - center_x
	perp_slope_num = int(slope_den)
	perp_slope_den = int(slope_num)
	# The point [height] away from point1 on the line between point1 and center
	inner_point = [int(center_x + inner_rad * math.cos(d)), int(center_x - inner_rad * math.sin(d))]
	# The two base points of the triangle, each equidistant from inner_point, forming a perpendicular to the line between point1 and center
	point2 = [inner_point[0] + perp_slope_den/10, inner_point[1] - perp_slope_num/10]
	point3 = [inner_point[0] - perp_slope_den/10, inner_point[1] + perp_slope_num/10]


	pygame.draw.polygon(screen, (0, 30, 140), (point1, point2, point3))


size_slider_surf = pygame.surface.Surface((145, 25))

pygame.draw.line(size_slider_surf, (127, 127, 0), (20, 20), (140, 20))

for i in range(5):
	x, y = 20 + i * 30 , 20
	pygame.draw.circle(size_slider_surf, (127, 127, 0), (x, y), 5)

window_resize(screen_w)
d = 0

while True:
	for event in pygame.event.get():
		if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
			pygame.quit()
			quit()

		elif event.type == pygame.VIDEORESIZE:
			# doesn't change anything
			window_resize(screen_w)

		elif event.type == pygame.MOUSEBUTTONDOWN:
			# for window resize
			for i in range(5):
				x, y = 20 + i * 30 , 20
		
				if mouse_x >= x - 10 and mouse_x < x + 10 and mouse_y >= y - 10 and mouse_y < y + 10:
					screen_size_slider = i
					screen_w = screen_sizes[screen_size_slider]
					window_resize(screen_w)

			# for changeing the unit
			if unit_text_rect.collidepoint(mouse_x, mouse_y):
				if unit == 'mph':
					unit = 'kmph'
				else:
					unit = 'mph'

	mouse_x, mouse_y = pygame.mouse.get_pos()
	d += 0.125 * clock.tick()

	screen.fill((0, 0, 0))

	screen.blit(size_slider_surf, (0, 0))
	pygame.draw.circle(screen, (0, 0, 127), (20 + screen_size_slider * 30, 20), 5)
	draw_compass()
	draw_windspeed(50, unit)
	draw_arrow(d)

	pygame.display.update()
