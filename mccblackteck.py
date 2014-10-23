#-*- coding: utf-8 -*-
import urllib2
import re


def get_request(url, content='', cookie=None):
    headers = {'UserAgent': 'Mozilla/4.0 ',
               'Accept-Encoding': 'utf-8',
               'Accept-Language': 'zh-CN',
               'Referer': 'http://civitas.soobb.com'}
    if not cookie is None:
        headers['cookie'] = cookie
    request = urllib2.Request(url, headers=headers)
    try:
        response = urllib2.urlopen(request, content)
        return {'status': True, 'exception': None, 'info': response.info(), 'content': response.read()}
    except Exception, ex:
        return {'status': False, 'exception': ex}


def login(email, password):
    result = get_request('http://www.soobb.com/Accounts/AjaxAuthenticate/',
                         content='Email=' + email + '&Password=' + password)
    if 'Set-cookie' in result['info']:
        return result['info']['Set-cookie']
    return


def regex_find(r, content):
    result = []
    p = re.compile(r)
    for m in p.finditer(content):
        result.append(m.group())
    return result


def try_get_value(dict, key):
    return dict[key] if key in dict else ''


def write_file(file, mode, string):
    output = open(file, mode)
    output.write(string + '\n')
    output.close()
    print string
