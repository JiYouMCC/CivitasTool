#-*- coding: utf-8 -*-
import re
from bs4 import BeautifulSoup
from mccblackteck import get_request, login, regex_find, DOMAIN, tryutf8
from account import EMAIL, PWD
import threading
import xlwt
import sys
import os

WORKS = {}
threadLock = threading.Lock()
THREAD_SIZE = 50
page_count = None
FINISH = 0
WIDTH = 68
RETRY = 5


class getWorks (threading.Thread):

    def __init__(self, page_num, cookie):
        threading.Thread.__init__(self)
        self.page_num = page_num
        self.cookie = cookie

    def run(self):
        global FINISH
        global page_count
        for i in range(RETRY):
            try:
                result = get_request(
                    '%s/Forums/Statistics/?Page=%s' % (DOMAIN, self.page_num), cookie=self.cookie)
                if result['status']:
                    content = result['content']
                    content = tryutf8(content)
                    soup = BeautifulSoup(content)
                    status_rows = soup.find_all('div', class_='StatisticsRow')
                    for status in status_rows:
                        b = BeautifulSoup(str(status))
                        b_tag = b.find_all('p', class_='Number')
                        works = []
                        if len(b_tag) == 9:
                            works = [b.string for b in b_tag]
                            works[0] = works[0][1:-1]
                            threadLock.acquire()
                            WORKS[b_tag[0].string] = works
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
    if not os.path.exists('works'):
        os.makedirs('works')
    # get pageCount
    result = get_request(
        "%s/Forums/Statistics/" % (DOMAIN), cookie=cookie)
    if result['status']:
        content = result['content']
        pagination = BeautifulSoup(content).find_all(
            'div', class_='Pagination')
        if pagination:
            page_count = int(
                BeautifulSoup(str(pagination[0])).find_all('a', href=True)[-2].string)
    threads = []
    for i in range(1, page_count + 1):
        threads.append(getWorks(i, cookie))
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
    for k, work in WORKS.iteritems():
        if i == 0:
            sheet += 1
            table = file.add_sheet(str(sheet), cell_overwrite_ok=True)
            table.write(i, 0, '天数')
            table.write(i, 1, '工作人口')
            table.write(i, 2, '副业劳工')
            table.write(i, 3, '总产能')
            table.write(i, 4, '总贸易额')
            table.write(i, 5, '活跃人口')
            table.write(i, 6, '工作机会')
            table.write(i, 7, '已垦土地')
            table.write(i, 8, '新垦土地')
            i += 1
        table.write(i, 0, int(work[0]))
        table.write(i, 1, int(work[1]))
        table.write(i, 2, int(work[2]))
        table.write(i, 3, float(work[3]))
        table.write(i, 4, float(work[4]))
        table.write(i, 5, int(work[5]))
        table.write(i, 6, int(work[6]))
        table.write(i, 7, float(work[7]))
        table.write(i, 8, float(work[8]))
        i += 1
        if i == 65535:
            i = 0
    file.save(os.path.join('works', 'works.xls'))
