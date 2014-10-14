#-*- coding: utf-8 -*-
import urllib2
import re
EMAIL = ''
PWD = ''


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

cookie = login(EMAIL, PWD)
if cookie is None:
    print "Login fail"
else:
    write_file('estate.txt', 'w+',
               'ID;城市;城区;地形;街区;状态;类型;名称;管理;拥有;产品;产品等级;建设;规划;占地;工作;居住;容纳;已工作;维护;开发')
    for i in range(1, 50000):
        result = get_request(
            'http://civitas.soobb.com/Estates/' + str(i) + '/Details/', cookie=cookie)
        if result['status']:
            content = result['content']
            estateName = estateType = estateStatus = estateLocation = estatePosition = estateManage = estateOwner = estateDistricts = estateProduct = estateProductLevel = estateCity = ''
            estateDevelop = estateArea = estatePeople = {}
            # Check ID
            idList = regex_find(
                r'<div class=\"Avatar AvatarMedium\"><a href=\"/Estates/[0-9]+/Details/\" class=\"Normal\">', content)
            if len(idList) == 1:
                id = re.compile(
                    r'(<div class=\"Avatar AvatarMedium\"><a href=\"/Estates/)([0-9]+)(/Details/\" class=\"Normal\">)').sub(r'\2', idList[0])
                if str(id) != str(i):
                    continue
            # Estate name
            nameList = regex_find(
                r'<a href="/Estates/[0-9]+/Details/">.+</a>', content)
            if len(nameList) == 1:
                estateName = re.compile(
                    r'<a href="/Estates/[0-9]+/Details/">(.+)</a>').sub(r'\1', nameList[0])
            if estateName == '':
                estateStatus = '消失'
            # Estate type and status
            typeList = regex_find(r'</a><span>\(.+\)</span></h3>', content)
            if len(typeList) == 1:
                typeAndStatus = re.compile(
                    r'(</a><span>\()(.+)(\)</span></h3>)').sub(r'\2', typeList[0])
                estateType = re.compile(
                    r'(.+) - (.+)中').sub(r'\1', typeAndStatus)
                estateStatus = re.compile(
                    r'(.+) - (.+)中').sub(r'\2', typeAndStatus)
                if estateStatus == estateType:
                    estateStatus = '正常'
            # Estate location
            locationList = regex_find(
                r'<span>&gt; 位于<a href="javascript:void\(0\)" class="WithEntityCard" entityid="[0-9]+">.+</a>.+</span>', content)
            if len(locationList) == 1:
                compileResult = re.compile(
                    r'<span>&gt; 位于<a href="javascript:void\(0\)" class="WithEntityCard" entityid="[0-9]+">(.+)</a>(.+)的(.+)</span>')
                estateCity = compileResult.sub(r'\1', locationList[0])
                estateLocation = compileResult.sub(r'\2', locationList[0])
                estatePosition = compileResult.sub(r'\3', locationList[0])
            # Estate manage
            manageList = regex_find(
                r'<span>&gt; <a href=".+>.+</a> 管理</span>', content)
            if len(manageList) == 1:
                estateManage = re.compile(
                    r'<span>&gt; <a href=".+>(.+)</a> 管理</span>').sub(r'\1', manageList[0])
            # Estate owner
            ownerList = regex_find(
                r'<span>&gt; <a href=".+>.+</a> 所有</span>', content)
            if len(ownerList) == 1:
                estateOwner = re.compile(
                    r'<span>&gt; <a href=".+>(.+)</a> 所有</span>').sub(r'\1', ownerList[0])
            # Estate Product
            productList = regex_find(r'<p>生产.+ \(等级[0-5]\)</p>', content)
            if len(productList) == 1:
                compileResult = re.compile(r'<p>生产(.+) \(等级([0-5])\)</p>')
                estateProduct = compileResult.sub(r'\1', productList[0])
                estateProductLevel = compileResult.sub(r'\2', productList[0])
            # Estate Districts
            districtsList = regex_find(
                r'<p>.+</p>[\w\W]{0,10}</div>[\w\W]{0,10}<div class=\"Brand\">[\w\W]{0,10}<div class=\"Name\">所属街区</div>', content)
            if len(districtsList) == 1:
                estateDistricts = re.compile(
                    r'<p>(.+)</p>[\w\W]*</div>[\w\W]*<div class=\"Brand\">[\w\W]*<div class=\"Name\">所属街区</div>'	).sub(r'\1', districtsList[0])
            # Estate Develop
            developList = regex_find(
                r'<div class=\"Name\">.+程度</div>[\w\W]{0,10}<div class=\"Badge\"><span class=\"Number\">[0-9]+[\.[0-9]*]?%</span></div>', content)
            for develop in developList:
                compileResult = re.compile(
                    r'<div class=\"Name\">(.+)程度</div>[\w\W]{0,10}<div class=\"Badge\"><span class=\"Number\">([0-9]+[\.[0-9]*]?%)</span></div>')
                estateDevelop[compileResult.sub(r'\1', develop)] = compileResult.sub(
                    r'\2', develop)
            if len(ownerList) == 1:
                estateOwner = re.compile(
                    r'<span>&gt; <a href=".+>(.+)</a> 所有</span>').sub(r'\1', ownerList[0])
            # Estate area
            areaList = regex_find(
                r'<p class=\"Number\">[0-9]+[\.[0-9]*]?</p>[\w\W]{0,10}<p class=\"Tips\">.+面积</p>', content)
            for area in areaList:
                compileResult = re.compile(
                    r'<p class=\"Number\">([0-9]+[\.[0-9]*]?)</p>[\w\W]{0,10}<p class=\"Tips\">(.+)面积</p>')
                estateArea[compileResult.sub(r'\2', area)] = compileResult.sub(
                    r'\1', area)
            # Estate people
            peopeList = regex_find(
                r'<p class=\"Number\">[0-9]+</p>[\w\W]{0,10}<p class=\"Tips\">.+人数</p>', content)
            for people in peopeList:
                compileResult = re.compile(
                    r'<p class=\"Number\">([0-9]+)</p>[\w\W]{0,10}<p class=\"Tips\">(.+)人数</p>')
                estatePeople[compileResult.sub(r'\2', people)] = compileResult.sub(
                    r'\1', people)
            write_file('estate.txt', 'a', str(i) + ';' +
                       estateCity + ';' +
                       estateLocation + ';' +
                       estatePosition + ';' +
                       estateDistricts + ';' +
                       estateStatus + ';' +
                       estateType + ';' +
                       estateName + ';' +
                       estateManage + ';' +
                       estateOwner + ';' +
                       estateProduct + ';' +
                       estateProductLevel + ';' +
                       try_get_value(estateArea, '建设',) + ';' +
                       try_get_value(estateArea, '规划',) + ';' +
                       try_get_value(estateArea, '占地',) + ';' +
                       try_get_value(estatePeople, '工作',) + ';' +
                       try_get_value(estatePeople, '居住',) + ';' +
                       try_get_value(estatePeople, '容纳',) + ';' +
                       try_get_value(estatePeople, '已工作',) + ';' +
                       try_get_value(estateDevelop, '维护',) + ';' +
                       try_get_value(estateDevelop, '开发',))
