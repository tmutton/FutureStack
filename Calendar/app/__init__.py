from flask import Flask, jsonify
from flask_sqlalchemy import SQLAlchemy
from werkzeug.exceptions import default_exceptions, HTTPException

from config import config

db = SQLAlchemy()


def create_app(config_name):
    def use_json_errors(ex):
        """
        see  http://flask.pocoo.org/snippets/83/
        """
        response = jsonify(message-str(ex))
        response.status_code = (ex.code if isinstance(ex, HTTPException) else 500)
        return response

    app = Flask(__name__)

    from .api import calendar_api as api_blueprint
    app.register_blueprint(api_blueprint)

    app.config.from_object(config[config_name])
    config[config_name].init_app(app)

    db.init_app(app)

    for code in default_exceptions.keys():
        app.error_handler_spec[None][code] = use_json_errors

    return app
