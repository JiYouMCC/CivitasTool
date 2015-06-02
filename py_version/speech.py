#-*- coding: utf-8 -*-
import urllib2
import re
from bs4 import BeautifulSoup
from mccblackteck import get_request, login, regex_find, try_get_value, write_file, DOMAIN
from account import EMAIL, PWD
import threading

SPEECH_TYPE = 1
SPEECHES = []
threadLock = threading.Lock()


class getSpeech (threading.Thread):

    def __init__(self, page_num, cookie):
        threading.Thread.__init__(self)
        self.page_num = page_num
        self.cookie = cookie

    def run(self):
        global SPEECHES
        result = get_request(
            '%s/Forums/Speeches/?SpeechType=1&Page%s' % (DOMAIN, self.page_num), cookie=self.cookie)
        if result['status']:
            content = result['content']
            content = tryutf8(content)
            soup = BeautifulSoup(content)
            speech_items = soup.find_all('div', class_='Speech')
            for item_number in range(len(speech_items)):
                speech = {}
                b = BeautifulSoup(str(speech_items[item_number]))
                r_day = '演讲，第'.decode('utf-8')
                day_tag = b.find_all('p', text=re.compile(r'.+%s.+' % r_day))
                if day_tag:
                    day_tag = day_tag[0]
                    result = []
                    p = re.compile(r'[0-9]+')
                    for m in p.finditer(day_tag.string):
                        result.append(m.group())
                    speech['day'] = int(result[0])
                    speech['hour'] = int(result[1])
                    speech['minute'] = int(result[2])
                speechId = b.find_all('div', class_='Speech')[0]['speechid']
                speech['id'] = speechId
                author = tryutf8(
                    b.find_all('a', class_='WithEntityCard', href=True)[1].string)
                speech['author'] = author
                tag = b.find_all('p', class_='')[0]
                text = tag.text
                try:
                    text = text.encode('utf-8')
                except Exception, e:
                    pass
                speech['content'] = text
                href = b.find_all('a', class_='', href=True)
                if href:
                    for h in href:
                        textt = h.string

                        if textt[0] == '#':
                            tag_text = textt[1:-1]
                            try:
                                tag_text = tag_text.encode('utf-8')
                            except Exception, e:
                                pass
                            speech['tag'] = tag_text
                        elif textt[0] == '(':
                            link_text = textt[1:-1]
                            try:
                                link_text = link_text.encode('utf-8')
                            except Exception, e:
                                pass
                            if not 'links' in speech:
                                speech['links'] = []
                            speech['links'].append([link_text, h['href']])
                threadLock.acquire()
                SPEECHES.append(speech)
                threadLock.release()
        print '.',


def tryutf8(str):
    try:
        str = str.encode('utf-8')
    except Exception, e:
        pass
    return str

cookie = login(EMAIL, PWD)
if cookie is None:
    print "Login fail"
else:
    # get pageCount
    page_count = None
    result = get_request(
        "%s/Forums/Speeches/?SpeechType=%s" % (DOMAIN, SPEECH_TYPE), cookie=cookie)
    if result['status']:
        content = result['content']
        pagination = BeautifulSoup(content).find_all(
            'div', class_='Pagination')
        if pagination:
            page_count = int(
                BeautifulSoup(str(pagination[0])).find_all('a', href=True)[-2].string)
    threads = []
    for page_num in range(page_count):
        threads.append(getSpeech(page_num, cookie))
    for thread in threads:
        thread.start()
        while True:
            if(len(threading.enumerate()) < 5):
                break
    for thread in threads:
        thread.join()
    print len(SPEECHES)
    # for speech in SPEECHES:
    #     if 'day' in speech:
    #         print 'D%s %.2d:%.2d' % (speech['day'], speech['hour'], speech['minute']),
    #         print '[', speech['author'], ']', speech['content']
