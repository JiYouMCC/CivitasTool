#-*- coding: utf-8 -*-
import urllib2
import re
from bs4 import BeautifulSoup
from mccblackteck import get_request, login, regex_find, DOMAIN, tryutf8
from account import EMAIL, PWD
import threading
import xlwt
import sys
import os
reload(sys)
sys.setdefaultencoding('utf8')

SPEECH_TYPE = 1
SPEECHES = {}
threadLock = threading.Lock()
THREAD_SIZE = 50
page_count = None
FINISH = 0
WIDTH = 68


class getSpeech (threading.Thread):

    def __init__(self, page_num, cookie, speech_type=1):
        threading.Thread.__init__(self)
        self.page_num = page_num
        self.cookie = cookie
        self.speech_type = speech_type

    def run(self):
        global FINISH
        global page_count
        while True:
            try:
                result = get_request(
                    '%s/Forums/Speeches/?SpeechType=%s&Page=%s' % (
                        DOMAIN, self.speech_type, self.page_num),
                    cookie=self.cookie)
                if result['status']:
                    content = result['content']
                    content = tryutf8(content)
                    soup = BeautifulSoup(content)
                    speech_items = soup.find_all('div', class_='Speech')
                    for speech_item in speech_items:
                        speech = {}
                        b = BeautifulSoup(str(speech_item))
                        # day and time
                        day_sign_str = '本地演讲，第'.decode('utf-8')
                        day_tag = b.find_all(
                            'p', text=re.compile(r'%s.+' % day_sign_str))
                        if day_tag:
                            day_tag = day_tag[0]
                            result = regex_find(r'[0-9]+', day_tag.string)
                            # day
                            speech['day'] = int(result[0])
                            # hour
                            speech['hour'] = int(result[1])
                            # minute
                            speech['minute'] = int(result[2])
                        # speech id
                        speechId = b.find_all(
                            'div', class_='Speech')[0]['speechid']
                        speech['id'] = speechId
                        # author
                        speech['author'] = tryutf8(
                            b.find_all('a', class_='WithEntityCard', href=True)[1].string)
                        # content
                        speech['content'] = tryutf8(
                            b.find_all('p', class_='')[0].text)
                        # Links
                        hrefs = b.find_all('a', class_='', href=True)
                        if hrefs:
                            for href in hrefs:
                                href_text = href.string
                                # tag
                                if href_text[0] == '#':
                                    speech['tag'] = tryutf8(href_text[1:-1])
                                # link
                                elif href_text[0] == '(':
                                    if not 'links' in speech:
                                        speech['links'] = []
                                    speech['links'].append(
                                        [tryutf8(href_text[1:-1]), href['href']])
                        # like
                        like_tag = b.find_all(
                            'span', class_='Number', type='1')
                        if like_tag:
                            like_tag = like_tag[0]
                        speech['like'] = int(like_tag.string[1:-1])
                        # watch
                        watch_tag = b.find_all(
                            'span', class_='Number', type='3')
                        if watch_tag:
                            watch_tag = watch_tag[0]
                        speech['watch'] = int(watch_tag.string[1:-1])
                        # dislike
                        dislike_tag = b.find_all(
                            'span', class_='Number', type='2')
                        if dislike_tag:
                            dislike_tag = dislike_tag[0]
                        speech['dislike'] = int(dislike_tag.string[1:-1])
                        threadLock.acquire()
                        # print speechId, speech['content']
                        SPEECHES[speechId] = speech
                        threadLock.release()
                break
            except Exception, e:
                print self.page_num, e
        threadLock.acquire()
        FINISH += 1
        oks = "=" * int(float(FINISH) / page_count * WIDTH)
        fls = " " * (WIDTH - int(float(FINISH) / page_count * WIDTH))
        print "\r[%s%s]%.2f" % (oks, fls, (float(FINISH) / page_count * 100)), "%",
        threadLock.release()


cookie = login(EMAIL, PWD)
if cookie is None:
    print "Login fail"
else:
    if not os.path.exists('speech'):
        os.makedirs('speech')
    # get pageCount
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
    for i in range(1, page_count + 1):
        threads.append(getSpeech(i, cookie))
    for thread in threads:
        thread.start()
        while True:
            if(len(threading.enumerate()) < THREAD_SIZE):
                break
    for thread in threads:
        thread.join()
    i = 0
    sheet = 0
    file = xlwt.Workbook(encoding='utf-8')
    for k, speech in SPEECHES.iteritems():
        if i == 0:
            sheet += 1
            table = file.add_sheet(str(sheet), cell_overwrite_ok=True)
            table.write(i, 0, 'Id')
            table.write(i, 1, 'Day')
            table.write(i, 2, 'Time')
            table.write(i, 3, 'Author')
            table.write(i, 4, 'Like')
            table.write(i, 5, 'Watch')
            table.write(i, 6, 'Dislike')
            table.write(i, 7, 'Tag')
            table.write(i, 8, 'Links')
            table.write(i, 9, 'Content')
            i += 1
        table.write(i, 0, speech['id'])
        if 'day' in speech:
            table.write(i, 1, speech['day'])
        if 'hour' in speech:
            table.write(i, 2, '%.2d:%.2d' % (speech['hour'], speech['minute']))
        table.write(i, 3, speech['author'])
        table.write(i, 4, speech['like'])
        table.write(i, 5, speech['watch'])
        table.write(i, 6, speech['dislike'])
        if 'tag' in speech:
            table.write(i, 7, speech['tag'])
        if 'links' in speech:
            table.write(i, 8, str(['%s' % link[1]
                                   for link in speech['links']]))
        table.write(i, 9, speech['content'])
        i += 1
        if i == 65535:
            i = 0
    file.save(os.path.join('speech', 'speech.xls'))
