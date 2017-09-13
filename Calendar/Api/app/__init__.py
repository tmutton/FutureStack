from flask import Flask, jsonify
from flask_migrate import Migrate
from flask_sqlalchemy import SQLAlchemy
from werkzeug.exceptions import default_exceptions, HTTPException

from config import config

db = SQLAlchemy()
migrate = Migrate()


def create_app(config_name):
    def use_json_errors(ex):
        """
        see  http://flask.pocoo.org/snippets/83/
        """
        response = jsonify(message=str(ex))
        response.status_code = (ex.code if isinstance(ex, HTTPException) else 500)
        return response

    app = Flask(__name__)

    from app.api.calendar import ics_feed
    app.register_blueprint(ics_feed)
    from app.api.admin import calhelp
    app.register_blueprint(calhelp)

    app.config.from_object(config[config_name])
    config[config_name].init_app(app)

    db.init_app(app)
    migrate.init_app(app, db)

    # for code in default_exceptions.keys():
    #    app.error_handler_spec[None][code] = use_json_errors

    return app
