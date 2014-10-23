#-*- coding: utf-8 -*-
import urllib2
import re
from mccblackteck import get_request, login, regex_find
# try_get_value, write_file
from account import EMAIL, PWD

cookie = login(EMAIL, PWD)
district = {}
if cookie is None:
    print "Login fail"
else:
    result = get_request('http://civitas.soobb.com/Forums/', cookie=cookie)
    if result['status']:
        content = result['content']
        district_list = regex_find(
            r'<h5><a href=\"/Districts/[0-9]{1,4}/\">.+</a></h5>', content)
        for district_item in district_list:
            compileResult = re.compile(
                r'<h5><a href=\"/Districts/([0-9]{1,10})/\">(.+)</a></h5>')
            district[compileResult.sub(r'\1', district_item)] = compileResult.sub(
                r'\2', district_item)
    for key, value in district.iteritems():
        print key, value,
        result = get_request(
            'http://civitas.soobb.com/Districts/' + str(key) + '/Estates/',
            cookie=cookie)
        if result['status']:
            content = result['content']
            district_list = regex_find(
                r'<li><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=[0-9]{1,4}\">[0-9]{1,4}</a></li>[\w\W]{0,10}</ul>[\w\W]{0,10}<span class=\"Next\"><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=2\">后页 &gt;</a></span>', content)
        for district_item in district_list:
            compileResult = re.compile(
                r'<li><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=([0-9]{1,4})\">[0-9]{1,4}</a></li>[\w\W]{0,10}</ul>[\w\W]{0,10}<span class=\"Next\"><a href=\"/Districts/[0-9]{1,4}/Estates/\?Action=Search&Page=2\">后页 &gt;</a></span>')
            print compileResult.sub(r'\1', district_item)
