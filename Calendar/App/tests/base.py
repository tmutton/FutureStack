from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from unittest import TestCase

from config import config
from model.models import Base


class TestBase(TestCase):
    def setUp(self):
        self.engine = create_engine(config['testing'].SQLALCHEMY_DATABASE_URI)
        self.uow = sessionmaker(bind=self.engine)
        Base.metadata.create_all(self.engine)

    def tearDown(self):
        Base.metadata.drop_all(self.engine)


