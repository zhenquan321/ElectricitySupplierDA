# -*-coding:utf-8-*- 
__author__ = 'admin'

import pymongo as pm
import traceback
import datetime
import threading as th
import time

# mongo_url = 'mongodb://seedbo:SfNLP1225@119.254.210.251:27000/see'
mongo_url = 'mongodb://seedbo:SfNLP1225@211.154.6.146:28117/orectoryperdb'


class MongodbHelper:
    '''
    mongodb操作的类
    '''

    def __init__(self):
        pass

    IW2S_BaiduCommend = None
    IW2S_BaiduKeyword = None
    IW2S_BotRegister = None
    has_ini = False
    Default_mongodb = None

    @staticmethod
    def initialize_collection():
        if (MongodbHelper.has_ini):
            return
        client = pm.MongoClient(mongo_url)
        MongodbHelper.Default_mongodb = client.get_default_database()
        MongodbHelper.IW2S_BaiduCommend = MongodbHelper.Default_mongodb.get_collection('IW2S_BaiduCommend')
        MongodbHelper.IW2S_BotRegister = MongodbHelper.Default_mongodb.get_collection('IW2S_BotRegister')
        MongodbHelper.IW2S_BaiduKeyword = MongodbHelper.Default_mongodb.get_collection('IW2S_BaiduKeyword')
        MongodbHelper.IW2S_ImgSearchTask = MongodbHelper.Default_mongodb.get_collection('IW2S_ImgSearchTask')
        MongodbHelper.IW2S_SG_BaiduKeyword = MongodbHelper.Default_mongodb.get_collection('IW2S_SG_BaiduKeyword')
        MongodbHelper.IW2S_WB_BaiduCommend = MongodbHelper.Default_mongodb.get_collection('IW2S_WB_BaiduCommend')
        MongodbHelper.IW2S_WX_BaiduKeyword = MongodbHelper.Default_mongodb.get_collection('IW2S_WX_BaiduKeyword')
        MongodbHelper.IW2S_level1link = MongodbHelper.Default_mongodb.get_collection('IW2S_level1link')
        MongodbHelper.IW2S_SG_BaiduCommend = MongodbHelper.Default_mongodb.get_collection('IW2S_SG_BaiduCommend')
        MongodbHelper.IW2S_ImgSearchLink = MongodbHelper.Default_mongodb.get_collection('IW2S_ImgSearchLink')
        MongodbHelper.IW2S_SG_level1link = MongodbHelper.Default_mongodb.get_collection('IW2S_SG_level1link')
        MongodbHelper.IW2S_WX_level1link = MongodbHelper.Default_mongodb.get_collection('IW2S_WX_level1link')
        MongodbHelper.IW2S_WB_level1link = MongodbHelper.Default_mongodb.get_collection('IW2S_WB_level1link')
        MongodbHelper.IW2S_WX_BaiduCommend = MongodbHelper.Default_mongodb.get_collection('IW2S_SG_BaiduCommend')



        client.close()
        MongodbHelper.has_ini = True


def get_bots():
    '''
    读取bot信息，每分钟检查一次，发现有bot超过3分钟未注册，
    则清除掉该bot信息
    :return: 所有正在运行的bot列表
    '''
    bots = MongodbHelper.IW2S_BotRegister.find()
    return bots


def check_bots():
    '''
    将Bot注册的时间更当前时间比较，如果超过3分钟则记下_id，等下删除该bot的信息
    改一下C#的程序把bot的id也传上来，还有进程名字，结合关键词表，看看哪些在搜，哪些闲着
    然后更改status状态
    Bot状态 0：空闲， 1：忙碌， 2：脱机， 3：异常
    :return:
    '''
    while True:
        try:
            bots = get_bots()
            nowDatetime = datetime.datetime.utcnow()
            for bot in bots:
                try:
                    _id = bot['_id']
                    reg_time = bot['RegTime']
                    bot_id = bot['BotId']
                    ip = bot['IpAddress']
                    outtime = abs((nowDatetime - reg_time).total_seconds())

                    if (outtime > 600):  # 10分钟 废弃
                        MongodbHelper.IW2S_BotRegister.delete_one({'_id': _id})
                        print str.format('{3}  bot {0}#{1} on {2} is timeout. deleted!', _id, bot_id, ip,
                                         datetime.datetime.now()), nowDatetime, reg_time
                        continue
                    elif (outtime > 180):  # 3分钟 脱机
                        MongodbHelper.IW2S_BotRegister.update_one({'_id': _id}, {'$set': {'Status': 2}})
                        print str.format('{3}  bot {0}#{1} on {2} is offline', _id, bot_id, ip,
                                         datetime.datetime.now()), nowDatetime, reg_time
                        continue

                    # check_bot_isbusy(bot_id, _id)
                except Exception:
                    traceback.print_exc()
                    continue


        except Exception:
            traceback.print_exc()
        time.sleep(60)  # 60s


def check_bot_isbusy(bot_id, _id):
    '''
    Bot状态 0：空闲， 1：忙碌， 2：脱机， 3：异常
    '''
    bot_count = MongodbHelper.IW2S_BaiduCommend.find({'BotId': bot_id, 'BotStatus': 1, 'IsRemoved': False}).count()
    bot_count = bot_count + MongodbHelper.IW2S_BaiduKeyword. \
        find({'BotId': bot_id, 'BotStatus': 1, 'IsRemoved': False}).count()

    if (bot_count == 0):
        # 更改bot状态为0
        MongodbHelper.IW2S_BotRegister.update_one({'_id': _id}, {'$set': {'Status': 0}})
        print str.format('{0}  {1} is available', datetime.datetime.now(), bot_id)
        pass
    elif (bot_count > 0):
        # 更改bot状态为1
        MongodbHelper.IW2S_BotRegister.update_one({'_id': _id}, {'$set': {'Status': 1}})
        print str.format('{0}  {1} is busy', datetime.datetime.now(), bot_id)
        pass


def count_task_control():
    '''
    统计信息调度方法，每天0点30(北京时间)分统计数据并保存在单独的一个集合里
    每分钟统计一下，保留72小时的数据
    统计程序是一个，但是被不同的任务调度，并保存在不同的地方
    :return:
    '''
    while True:
        try:
            now = datetime.datetime.utcnow()
            out_time = now + datetime.timedelta(hours=-72)
            # 删掉过期数据
            r = MongodbHelper.Default_mongodb.IW2S_BotDataIn72Hours. \
                delete_many({'ins_time': {'$lt': out_time}})
            print str.format('delete {0} records in 72hours', r.deleted_count)
            # 插入新数据
            count_data_info('IW2S_BotDataIn72Hours')
        except Exception:
            traceback.print_exc()
        time.sleep(60)  # 60s


def count_task_control_byday():
    '''
    按天统计
    '''
    while True:
        try:
            utcnow = datetime.datetime.utcnow()
            print utcnow
            if (utcnow.hour == 16) and (utcnow.minute == 30):
                count_data_info('IW2S_BotDataInDay')
                group_baidu_commend()
                group_user()
                group_links()
                group_baidu_commend_endat()
                group_project()
        except Exception:
            traceback.print_exc()
        time.sleep(30)   # 30s


def count_data_info(collection_name):
    '''
    统计数据的逻辑在此，每天一次
    :return:
    '''
    data = {}
    data['users'] = MongodbHelper.Default_mongodb.IW2SUser.find({}).count()  # 总用户数
    user_ids = MongodbHelper.Default_mongodb.IW2SUser.find({}, {'_id': True})
    a_usrs = 0
    for id_d in user_ids:
        #c = MongodbHelper.Default_mongodb.IW2S_BaiduCommend.find({'UsrId': id_d['_id'], 'IsRemoved': False}).count()
        c = MongodbHelper.Default_mongodb.IW2S_BaiduKeyword.find({'UsrId': id_d['_id'], 'IsRemoved': False}).count()
        if (c > 0):
            a_usrs += 1
        else:
            c += MongodbHelper.Default_mongodb.IW2S_ImgSearchTask.find({'UsrId': id_d['_id'], 'IsDel': False}).count()
            if(c>0):
                a_usrs += 1
            else:
                c += MongodbHelper.Default_mongodb.IW2S_SG_BaiduKeyword.find({'UsrId': id_d['_id'], 'IsRemoved': False}).count()
                if(c > 0):
                    a_usrs += 1
                else:
                    c += MongodbHelper.Default_mongodb.IW2S_WB_BaiduCommend.find({'UsrId': id_d['_id'], 'IsRemoved': False}).count()
                    if(c > 0):
                        a_usrs += 1
                    else:
                        c += MongodbHelper.Default_mongodb.IW2S_WX_BaiduKeyword.find({'UsrId': id_d['_id'], 'IsRemoved': False}).count()
                        if (c > 0):
                            a_usrs += 1

    data['active_users'] = a_usrs  # 有关键词的用户（活动用户）
    data['projects'] = MongodbHelper.Default_mongodb.IW2S_Project.find({'IsDel': False}).count()  # 总项目数
    prj_ids = MongodbHelper.Default_mongodb.IW2S_Project.find({'IsDel': False}, {'_id': True})
    links,img_links,sg_links,wb_links,wx_links = 0,0,0,0,0
    a_prjs = 0
    for id_d in prj_ids:
        #c = MongodbHelper.Default_mongodb.IW2S_BaiduCommend.find({'ProjectId': id_d['_id'], 'IsRemoved': False}).count()
        c = MongodbHelper.Default_mongodb.IW2S_BaiduKeyword.find({'ProjectId': id_d['_id'], 'IsRemoved': False}).count()
        if (c > 0):
            a_prjs += 1
        else:
            c += MongodbHelper.Default_mongodb.IW2S_ImgSearchTask.find({'ProjectId': id_d['_id'], 'IsDel': False}).count()
            if (c > 0):
                a_prjs += 1
            else:
                c += MongodbHelper.Default_mongodb.IW2S_SG_BaiduKeyword.find(
                    {'ProjectId': id_d['_id'], 'IsRemoved': False}).count()
                if (c > 0):
                    a_prjs += 1
                else:
                    c += MongodbHelper.Default_mongodb.IW2S_WB_BaiduCommend.find(
                        {'ProjectId': id_d['_id'], 'IsRemoved': False}).count()
                    if (c > 0):
                        a_prjs += 1
                    else:
                        c += MongodbHelper.Default_mongodb.IW2S_WX_BaiduKeyword.find(
                            {'ProjectId': id_d['_id'], 'IsRemoved': False}).count()
                        if (c > 0):
                            a_prjs += 1
        b = MongodbHelper.Default_mongodb.IW2S_level1link.find({'ProjectId': id_d['_id']}).count()
        links += b
        img_links += MongodbHelper.Default_mongodb.IW2S_ImgSearchLink.find({'ProjectId': id_d['_id']}).count()
        sg_links += MongodbHelper.Default_mongodb.IW2S_SG_level1link.find({'ProjectId': id_d['_id']}).count()
        wx_links += MongodbHelper.Default_mongodb.IW2S_WX_level1link.find({'ProjectId': id_d['_id']}).count()
        wb_links += MongodbHelper.Default_mongodb.IW2S_WB_level1link.find({'ProjectId': id_d['_id']}).count()
    data['links'] = links  # 总链接数
    data['img_links'] = img_links
    data['sg_links']=sg_links
    data['wx_links']=wx_links
    data['wb_links']=wb_links

    data['active_projects'] = a_prjs  # 有关键词的项目（活动项目）
    data['keywords'] = MongodbHelper.IW2S_BaiduCommend.find({'IsRemoved': False}).count()  # 关键词数
    data['kw_sch'] = MongodbHelper.IW2S_BaiduCommend.find({'BotStatus': 1, 'IsRemoved': False}).count()  # 在搜的关键词数
    data['kw_complete'] = MongodbHelper.IW2S_BaiduCommend.find({'BotStatus': 2, 'IsRemoved': False}).count()  # 完成的关键词
    data['kw_wait'] = MongodbHelper.IW2S_BaiduCommend.find({'BotStatus': 0, 'IsRemoved': False}).count()  # 未搜的关键词
    data['ins_time'] = datetime.datetime.utcnow()  # 插入时间
    data['update_date'] = datetime.datetime(datetime.datetime.utcnow().year,
                                            datetime.datetime.utcnow().month,
                                            datetime.datetime.utcnow().day, 0, 0, 0, 0, None)  # 日期

    # data['links'] = MongodbHelper.Default_mongodb.IW2S_level1link.find({}).count()      # 总链接数

    # 微博
    data['wb_keywords'] = MongodbHelper.IW2S_WB_BaiduCommend.find({'IsRemoved': False}).count()  # 关键词数
    data['wb_kw_sch'] = MongodbHelper.IW2S_WB_BaiduCommend.find({'BotStatus': 1, 'IsRemoved': False}).count()  # 在搜的关键词数
    data['wb_kw_complete'] = MongodbHelper.IW2S_WB_BaiduCommend.find({'BotStatus': 2, 'IsRemoved': False}).count()  # 完成的关键词
    data['wb_kw_wait'] = MongodbHelper.IW2S_WB_BaiduCommend.find({'BotStatus': 0, 'IsRemoved': False}).count()  # 未搜的关键词

    # 百度图片
    data['img_keywords'] = MongodbHelper.IW2S_ImgSearchTask.find({'IsDel': False}).count()  # 关键词数
    data['img_kw_sch'] = MongodbHelper.IW2S_ImgSearchTask.find({'BotStatus': 1, 'IsDel': False}).count()  # 在搜的关键词数
    data['img_kw_complete'] = MongodbHelper.IW2S_ImgSearchTask.find({'BotStatus': 2, 'IsDel': False}).count()  # 完成的关键词
    data['img_kw_wait'] = MongodbHelper.IW2S_ImgSearchTask.find({'BotStatus': 0, 'IsDel': False}).count()  # 未搜的关键词
    # 搜狗
    data['sg_keywords'] = MongodbHelper.IW2S_SG_BaiduCommend.find({'IsRemoved': False}).count()  # 关键词数
    data['sg_kw_sch'] = MongodbHelper.IW2S_SG_BaiduCommend.find({'BotStatus': 1, 'IsRemoved': False}).count()  # 在搜的关键词数
    data['sg_kw_complete'] = MongodbHelper.IW2S_SG_BaiduCommend.find({'BotStatus': 2, 'IsRemoved': False}).count()  # 完成的关键词
    data['sg_kw_wait'] = MongodbHelper.IW2S_SG_BaiduCommend.find({'BotStatus': 0, 'IsRemoved': False}).count()  # 未搜的关键词
    # 微信
    data['wx_keywords'] = MongodbHelper.IW2S_WX_BaiduCommend.find({'IsRemoved': False}).count()  # 关键词数
    data['wx_kw_sch'] = MongodbHelper.IW2S_WX_BaiduCommend.find({'BotStatus': 1, 'IsRemoved': False}).count()  # 在搜的关键词数
    data['wx_kw_complete'] = MongodbHelper.IW2S_WX_BaiduCommend.find({'BotStatus': 2, 'IsRemoved': False}).count()  # 完成的关键词
    data['wx_kw_wait'] = MongodbHelper.IW2S_WX_BaiduCommend.find({'BotStatus': 0, 'IsRemoved': False}).count()  # 未搜的关键词


    MongodbHelper.Default_mongodb.get_collection(collection_name).insert_one(data)
    print datetime.datetime.now(), collection_name, data

def group_user():
    '''
    按CreateAt 分组汇用户，放在每天统计的方法中
    '''
    # MongodbHelper.initialize_collection()
    c = MongodbHelper.Default_mongodb.IW2S_User_group_create.find({}).count()
    pip = None
    if c == 0:
        pip = [{'$match': {'CreatedAt': {'$ne': None}}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]
    else:
        n = MongodbHelper.Default_mongodb.IW2S_User_group_create.find({}).sort('byday', -1).limit(1)
        lastday = n[0]['byday']
        lastday += datetime.timedelta(days=1)
        endday = datetime.datetime.strptime(str(datetime.date.today()), '%Y-%m-%d')
        pip = [{'$match': {'CreatedAt': {'$gte': lastday, '$lt': endday}}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]

    r = MongodbHelper.Default_mongodb.IW2SUser.aggregate(pip)
    #print list(r)
    if r.alive:
        ir = MongodbHelper.Default_mongodb. \
            IW2S_User_group_create.insert_many([{'byday': datetime.datetime.strptime(m['_id'], '%Y-%m-%d'),
                                                         'count': m['number']} for m in r])
        print datetime.datetime.now(), 'IW2SUser group by CreateAt', len(ir.inserted_ids)


def group_project():
    '''
    按CreateAt 分组汇项目，放在每天统计的方法中
    '''
    # MongodbHelper.initialize_collection()
    c = MongodbHelper.Default_mongodb.IW2S_Project_group_create.find({}).count()
    pip = None
    if c == 0:
        pip = [{'$match': {'IsDel': False}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]
    else:
        n = MongodbHelper.Default_mongodb.IW2S_Project_group_create.find({}).sort('byday', -1).limit(1)
        lastday = n[0]['byday']
        lastday += datetime.timedelta(days=1)
        endday = datetime.datetime.strptime(str(datetime.date.today()), '%Y-%m-%d')
        pip = [{'$match': {'IsDel': False, 'CreatedAt': {'$gte': lastday, '$lt': endday}}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]

    r = MongodbHelper.Default_mongodb.IW2S_Project.aggregate(pip)
    if r.alive:
        ir = MongodbHelper.Default_mongodb. \
            IW2S_Project_group_create.insert_many([{'byday': datetime.datetime.strptime(m['_id'], '%Y-%m-%d'),
                                                         'count': m['number']} for m in r])
        print datetime.datetime.now(), 'IW2S_Project group by CreateAt', len(ir.inserted_ids)


def group_baidu_commend():
    '''
    按CreateAt 分组汇总百度推荐词，放在每天统计的方法中
    '''
    # MongodbHelper.initialize_collection()
    c = MongodbHelper.Default_mongodb.IW2S_BaiduCommend_group_create.find({}).count()
    pip = None
    if c == 0:
        pip = [{'$match': {'IsRemoved': False}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]
    else:
        n = MongodbHelper.Default_mongodb.IW2S_BaiduCommend_group_create.find({}).sort('byday', -1).limit(1)
        lastday = n[0]['byday']
        lastday += datetime.timedelta(days=1)
        endday = datetime.datetime.strptime(str(datetime.date.today()), '%Y-%m-%d')
        pip = [{'$match': {'IsRemoved': False, 'CreatedAt': {'$gte': lastday, '$lt': endday}}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]

    r = MongodbHelper.Default_mongodb.IW2S_BaiduCommend.aggregate(pip)
    if r.alive:
        ir = MongodbHelper.Default_mongodb. \
            IW2S_BaiduCommend_group_create.insert_many([{'byday': datetime.datetime.strptime(m['_id'], '%Y-%m-%d'),
                                                         'count': m['number']} for m in r])
        print datetime.datetime.now(), 'IW2S_BaiduCommend group by CreateAt', len(ir.inserted_ids)

def group_baidu_commend_endat():
    '''
    LastBotEndAt 分组汇总百度推荐词，放在每天统计的方法中
    '''
    # MongodbHelper.initialize_collection()
    c = MongodbHelper.Default_mongodb.IW2S_BaiduCommend_group_end.find({}).count()
    pip = None
    if c == 0:
        pip = [{'$match': {'IsRemoved': False, 'BotStatus': 2}},
               {'$project': {'day': {'$substr': ['$LastBotEndAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]
    else:
        n = MongodbHelper.Default_mongodb.IW2S_BaiduCommend_group_end.find({}).sort('byday', -1).limit(1)
        lastday = n[0]['byday']
        lastday += datetime.timedelta(days=1)
        endday = datetime.datetime.strptime(str(datetime.date.today()), '%Y-%m-%d')
        pip = [{'$match': {'IsRemoved': False, 'BotStatus': 2,
                           'LastBotEndAt': {'$gte': lastday, '$lt': endday}}},
               {'$project': {'day': {'$substr': ['$LastBotEndAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]

    r = MongodbHelper.Default_mongodb.IW2S_BaiduCommend.aggregate(pip)
    if r.alive:
        ir = MongodbHelper.Default_mongodb. \
            IW2S_BaiduCommend_group_end.insert_many([{'byday': datetime.datetime.strptime(m['_id'], '%Y-%m-%d'),
                                                         'count': m['number']} for m in r])
        print datetime.datetime.now(), 'IW2S_BaiduCommend group by LastBotEndAt', len(ir.inserted_ids)

def group_links():
    '''
    CreatedAt 分组汇总IW2S_level1link，放在每天统计的方法中
    '''
    #MongodbHelper.initialize_collection()
    c = MongodbHelper.Default_mongodb.IW2S_level1link_group_create.find({}).count()
    pip = None
    if c == 0:
        pip = [
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]
    else:
        n = MongodbHelper.Default_mongodb.IW2S_level1link_group_create.find({}).sort('byday', -1).limit(1)
        lastday = n[0]['byday']
        lastday += datetime.timedelta(days=1)
        endday = datetime.datetime.strptime(str(datetime.date.today()), '%Y-%m-%d')
        pip = [{'$match': {'CreatedAt': {'$gte': lastday, '$lt': endday}}},
               {'$project': {'day': {'$substr': ['$CreatedAt', 0, 10]}}},
               {'$group': {'_id': '$day', 'number': {'$sum': 1}}},
               {'$sort': {'_id': 1}}]

    r = MongodbHelper.Default_mongodb.IW2S_level1link.aggregate(pip)
    if r.alive:
        ir = MongodbHelper.Default_mongodb. \
            IW2S_level1link_group_create.insert_many([{'byday': datetime.datetime.strptime(m['_id'], '%Y-%m-%d'),
                                                      'count': m['number']} for m in r])
        print datetime.datetime.now(), 'IW2S_level1link group by CreateAt', len(ir.inserted_ids)


def main():
    MongodbHelper.initialize_collection()

    th_checkbots = th.Thread(target=check_bots)
    th_task_control = th.Thread(target=count_task_control)
    th_task_byday = th.Thread(target=count_task_control_byday)

    th_checkbots.start()
    th_task_control.start()
    th_task_byday.start()


def test():
    now = datetime.datetime.utcnow()
    print now.year, now.month, now.day, now.hour, now.minute, now.second
    MongodbHelper.initialize_collection()
    col = MongodbHelper.Default_mongodb.iwsf.find().count()
    print col

    pass


def test_ins():
    MongodbHelper.initialize_collection()
    while True:
        now = datetime.datetime.utcnow()
        out_time = now + datetime.timedelta(seconds=-72)
        MongodbHelper.Default_mongodb. \
            IW2S_BotDataIn72Hours.delete_many({'ins_time': {'$lt': out_time}})

        MongodbHelper.Default_mongodb.IW2S_BotDataIn72Hours.insert_one({'ins_time': now})
        print now
        time.sleep(1)


if (__name__ == '__main__'):
    # test()


    main()
    # test_ins()
    # start = datetime.datetime.strptime('2016-05-15 1:13', '%Y-%m-%d %H:%M')
    # end = datetime.datetime.strptime('2016-05-16 1:13', '%Y-%m-%d %H:%M')
    #
    # MongodbHelper.initialize_collection()
    # data = MongodbHelper.Default_mongodb.IW2S_BotDataIn72Hours.find({'ins_time': {'$gte': start, '$lte': end}})\
    #     .sort('ins_time', 1).limit(1)
    # # data = MongodbHelper.Default_mongodb.IW2S_BotDataIn72Hours.find({}).sort('ins_time', -1).limit(1)
    # for d in data:
    #     print d
    # print data
