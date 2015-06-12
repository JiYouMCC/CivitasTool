#-*- coding: utf-8 -*-
import re
from bs4 import BeautifulSoup
from mccblackteck import get_request, login, regex_find, DOMAIN, tryutf8, print_processbar
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
                    content = tryutf8(result['content'])
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
        print_processbar(page_count, FINISH)
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
    title = ['天数', '工作人口', '副业劳工', '总产能',
             '总贸易额', '活跃人口', '工作机会', '已垦土地', '新垦土地']
    for k, work in WORKS.iteritems():
        if i == 0:
            sheet += 1
            table = file.add_sheet(str(sheet), cell_overwrite_ok=True)
            for j in range(9):
                table.write(i, j, title[j])
            i += 1
        for j in range(9):
            table.write(i, j, float(work[0]))
        i += 1
        if i == 65535:
            i = 0
    file.save(os.path.join('works', 'works.xls'))
