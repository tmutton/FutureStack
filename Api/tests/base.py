# project/tests/base.py


from flask_testing import TestCase

from app import create_app, db
from config import config

app = create_app('testing')


class BaseTestCase(TestCase):
    def create_app(self):
        return app

    def setUp(self):
        db.create_all()
        db.session.commit()

    def tearDown(self):
        db.session.remove()
        db.drop_all()