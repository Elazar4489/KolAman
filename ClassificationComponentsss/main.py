import os
from confluent_kafka import Consumer, KafkaError
import json
import datetime
from dotenv import load_dotenv
import math
# import pandas as pd
import redis
# from redis import aggregation as aggregations
# from redis import reducers as reducers
# from redis import TextField, NumericField, TagField
# from redis import IndexDefinition, IndexType
# from redis.commands.search.query import Query
# import redis.exceptions
r = redis.Redis(host='localhost', port=6379, decode_responses=True)
schema = (
    r.TextField("$.name", as_name="name"),
    r.TagField("$.city", as_name="city"),
    r.NumericField("$.age", as_name="age")
)
x=0

indexCreated = r.ft("idx:users").create_index(
    schema,
    definition=r.IndexDefinition(
        prefix=["user:"], index_type=r.IndexType.JSON
    )
)
# r.set('foo', 'bar')
# r.get('foo')

# r.hset('user-session:123', mapping={
#     'name': 'John',
#     "surname": 'Smith',
#     "company": 'Redis',
#     "age": 29,
# })

# r.hgetall('user-session:123')
# r.close()

load_dotenv()
bootstrap_servers = os.getenv("KAFKA_BROKER")
topic = os.getenv("TOPIC")
group_id = os.getenv("GROUP_ID")
auto_offset_reset = os.getenv("AUTO_OFFSET_RESET")
enable_auto_commit = os.getenv("ENABLE_AUTO_COMMIT")
client_id = os.getenv("CLIENT_ID")

consumer_conf = {
    'bootstrap.servers': bootstrap_servers,
    'group.id': group_id,
    'auto.offset.reset': auto_offset_reset,
    'enable.auto.commit': enable_auto_commit
}
consumer = Consumer(consumer_conf)
consumer.subscribe([topic])


def consume_raw_data():
    try:
        while True:
            msg = consumer.poll(timeout=1.0)

            if msg is None:
                continue
            else:
                if msg.error():
                    if msg.error().code() != KafkaError._PARTITION_EOF:
                        print(f"Consumer error: {msg.error()}")
                else:
                    try:
                        raw_line = msg.value().decode('utf-8').strip()
                        data = json.loads(raw_line)
                        print(f"message number {data['event_id']} was read successfully.")
                        enter_to_redis(data)
                        if validate(data):
                            geographical_classification()
                            distributing_notifications_to_commands()
                            writing_and_saving_logs()
                    except json.JSONDecodeError:
                        print(f"Malformed JSON: {msg.value()}")
    # except KeyboardInterrupt:
    #     print(r)
    except KafkaError:
        print("kafka")
    except BufferError:
        print("Buffer")

def enter_to_redis(message):
    x+=1
    r.json().set(f"user:{x}", r.Path.root_path(), message)
    return None


def geographical_classification():
    pass

def validate():
    pass

def distributing_notifications_to_commands():
    pass

def writing_and_saving_logs():
    pass


# import redis
# r = redis.Redis(host='localhost', port=6379, decode_responses=True)
# r.set('foo', 'bar')
# # True
# r.get('foo')
# # bar

# r.hset('user-session:123', mapping={
#     'name': 'John',
#     "surname": 'Smith',
#     "company": 'Redis',
#     "age": 29,
# })
# # True

# r.hgetall('user-session:123')
# # {'surname': 'Smith', 'name': 'John', 'company': 'Redis', 'age': '29'}

# r.close()









# #!/usr/bin/env python
# import pika

# connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
# channel = connection.channel()

# channel.queue_declare(queue='hello', durable=True, arguments={'x-queue-type': 'quorum'})

# channel.basic_publish(exchange='',
#                       routing_key='hello',
#                       body='Hello World!')
# print(" [x] Sent 'Hello World!'")

# connection.close()