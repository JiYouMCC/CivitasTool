#-*- coding: utf-8 -*-
import urllib2
import re
from mccblackteck import tryutf8, get_request, login, regex_find, DOMAIN, write_file
from account import EMAIL, PWD
import datetime
import os
import xlwt
from bs4 import BeautifulSoup

cookie = login(EMAIL, PWD)
district = []
if cookie is None:
    print "Login fail"
else:
    square_result = get_request(
        DOMAIN + '/Forums/', cookie=cookie)
    if square_result['status']:
        square_content = square_result['content']
        soup = BeautifulSoup(square_content)
        # city
        city_name = tryutf8(
            soup.find_all('title')[0].string.split('的广场'.decode('utf-8'))[0])
        # day
        day_number = int(regex_find(
            r'[0-9]+', BeautifulSoup(str(soup.find_all('div', class_='Clock')[0])).find_all('p')[1].string)[0])
        soup_districts = soup.find_all('div', class_='District', href=True)
        for district_item in soup_districts:
            soup_district_tag = BeautifulSoup(str(district_item)).find_all(
                'a', class_=False, href=True)[0]
            district.append([int(regex_find(
                r'[0-9]+', str(soup_district_tag))[0]),  tryutf8(soup_district_tag.string)])
    for district_item in district:
        district_id = district_item[0]
        district_name = district_item[1]
        print district_id, district_name,
        estates = []
        district_result = get_request(
            DOMAIN + '/Districts/' + str(district_id) + '/Estates/',
            cookie=cookie)
        if district_result['status']:
            district_content = district_result['content']
            district_list = regex_find(
                r'<li><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=[0-9]{1,4}\">[0-9]{1,4}</a></li>[\w\W]{0,10}</ul>[\w\W]{0,10}<span class=\"Next\"><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=2\">后页 &gt;</a></span>[\w\W]{0,10}<span class=\"Count\">[\w\W]共 [0-9]{1,4} 条[\w\W]</span>', district_content)
        pagecount, estatecount = None, None
        for district_item in district_list:
            compileResult = re.compile(
                r'<li><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=([0-9]{1,4})\">[0-9]{1,4}</a></li>[\w\W]{0,10}</ul>[\w\W]{0,10}<span class=\"Next\"><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=2\">后页 &gt;</a></span>[\w\W]{0,10}<span class=\"Count\">[\w\W]共 ([0-9]{1,4}) 条[\w\W]</span>')
            pagecount = compileResult.sub(r'\1', district_item)
            estatecount = compileResult.sub(r'\2', district_item)
        if not pagecount:
            pagecount = 1
        if not estatecount:
            estatecount = "少于一页，不高兴数有几"
        print pagecount, '页', estatecount, '条'
        for page in range(1, int(pagecount) + 1):
            page_result = get_request(
                DOMAIN + '/Districts/' +
                str(district_id) + '/Estates/?Action=Search&Page=' +
                str(page),
                cookie=cookie)
            if page_result['status']:
                page_content = page_result['content']
                estate_list = regex_find(
                    '<div class=\"Avatar\">[\w\W]{1,2000}产业影响', page_content)
                for estate in estate_list:
                    compileResult = re.compile(
                        r'[\w\W]+<h5><a href=\"(/Estates/[0-9]{1,10}/Details/)\">(.+)</a></h5>[\w\W]+')
                    estate_link = compileResult.sub(r'\1', estate)
                    name = compileResult.sub(r'\2', estate)
                    compileResult = re.compile(
                        r'[\w\W]+<div><a href=\"(.+)\" class="WithEntityCard" entityid=\"[0-9]{1,10}\">(.+)</a>的(.+)</div>[\w\W]+')
                    owner_link = compileResult.sub(r'\1', estate)
                    owner = compileResult.sub(r'\2', estate)
                    etype = compileResult.sub(r'\3', estate)
                    compileResult = re.compile(
                        r'[\w\W]+<p class=\"Number\">(.{1,10})</p>[\w\W]{0,10}<p class="Tips">占地面积[\w\W]+')
                    area = compileResult.sub(r'\1', estate)
                    estates.append(
                        [DOMAIN + estate_link, name, DOMAIN + owner_link, owner, etype, area])
        output_string = '<table><tr><td>' + str(district_id) + '</td><td>' + \
            district_name + '</td><td>' + \
            str(datetime.datetime.now()) + '</td></tr>'
        output_string = output_string + \
            '<tr><td>不动产名称</td><td>不动产主人</td><td>类型</td><td>面积</td></tr>'
        for estate in estates:
            for item in estate:
                print item,
            print
            output_string = output_string + '<tr><td><a href=\"' + estate[
                0] + '\">' + estate[
                    1] + '</a></td><td><a href=\"' + estate[
                        2] + '\">' + estate[
                            3] + '</a></td><td>' + estate[
                                4] + '</td><td>' + str(estate[
                                    5]) + '</td></tr>'
        output_string = output_string + '</table>'
        if not os.path.exists('districts'):
            os.makedirs('districts')
        if not os.path.exists(os.path.join('districts', city_name.decode("utf-8"))):
            os.makedirs(os.path.join('districts', city_name.decode("utf-8")))
        write_file(os.path.join('districts', city_name.decode("utf-8"),
                                "D%s_%s_%s_%s.xml" % (day_number, datetime.datetime.now().hour, datetime.datetime.now().minute, district_name.decode("utf-8"))), 'a', output_string)
