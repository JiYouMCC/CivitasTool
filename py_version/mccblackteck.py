#-*- coding: utf-8 -*-
import urllib2
import re

DOMAIN = 'http://civitas.soobb.com'

RETRY = 10
WIDTH = 68


def get_request(url, content='', cookie=None):
    headers = {'UserAgent': 'Mozilla/4.0 ',
               'Accept-Encoding': 'utf-8',
               'Accept-Language': 'zh-CN',
               'Referer': DOMAIN}
    if not cookie is None:
        headers['cookie'] = cookie
    request = urllib2.Request(url, headers=headers)
    response = None
    ex = None
    for i in range(RETRY):
        try:
            response = urllib2.urlopen(request, content)
            return {'status': True,
                    'exception': None,
                    'info': response.info(),
                    'content': response.read()}
        except Exception, e:
            ex = e
    return {'status': False,
            'exception': ex}


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


def tryutf8(str):
    try:
        str = str.encode('utf-8')
    except Exception, e:
        pass
    return str


def print_processbar(now, finish):
    oks = "=" * int(float(finish) / now * WIDTH)
    fls = " " * (WIDTH - int(float(finish) / now * WIDTH))
    print "\r[%s%s]%.2f" % (oks, fls, (float(finish) / now * 100)), "%",
