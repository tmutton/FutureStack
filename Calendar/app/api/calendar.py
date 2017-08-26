from . import api


@api.route('/calendar', methods=['GET', 'POST'])
def index():
    return '<h1>Hello World!</h1>'

