import logging
import os
import sys
import time
from multiprocessing import Queue

from arame.gateway import ArameConsumer
from brightside.connection import Connection
from brightside.command_processor import CommandProcessor, Request
from brightside.dispatch import ConsumerConfiguration, Dispatcher
from brightside.messaging import BrightsideConsumerConfiguration, BrightsideMessage
from brightside.registry import Registry
from arame.messaging import JsonRequestSerializer
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker

from config import config
from ports.handlers import ToDoCreated, ToDoCreatedEventHandler

KEYBOARD_INTERRUPT_SLEEP = 3    # How long before checking for a keyboard interrupt

logging.basicConfig(stream=sys.stdout, level=logging.DEBUG)


def command_processor_factory(channel_name:str):

    engine = create_engine(config[os.getenv('WORKER_CONFIG') or 'default'].SQLALCHEMY_DATABASE_URI)

    uow = sessionmaker(bind=engine)

    handler = ToDoCreatedEventHandler(uow)

    subscriber_registry = Registry()
    subscriber_registry.register(ToDoCreated, lambda: handler)

    command_processor = CommandProcessor(
        registry=subscriber_registry
    )
    return command_processor


def consumer_factory(connection: Connection, consumer_configuration: BrightsideConsumerConfiguration, logger: logging.Logger):
    return ArameConsumer(connection=connection, configuration=consumer_configuration, logger=logger)


def map_my_command_to_request(message: BrightsideMessage) -> Request:
    return JsonRequestSerializer(request=ToDoCreated(), serialized_request=message.body.value).deserialize_from_json()


def run():
    pipeline = Queue()
    amqp_uri = os.getenv('BROKER')
    connection = Connection(amqp_uri, "future.stack.exchange", is_durable=False)
    configuration = BrightsideConsumerConfiguration(pipeline, "taskcreated.event", "taskcreated.event")
    consumer = ConsumerConfiguration(connection, configuration, consumer_factory, command_processor_factory, map_my_command_to_request)
    dispatcher = Dispatcher({"ToDoCreatedEvent": consumer})

    dispatcher.receive()

    # poll for keyboard input to allow the user to quit monitoring
    while True:
        try:
            # just sleep unless we receive an interrupt i.e. CTRL+C
            time.sleep(KEYBOARD_INTERRUPT_SLEEP)
        except KeyboardInterrupt:
            dispatcher.end()
            sys.exit(1)


if __name__ == "__main__":
    run()
