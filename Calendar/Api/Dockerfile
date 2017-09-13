FROM python:3.6.1
ENV PYTHONUNBUFFERED 1

RUN mkdir /code
ADD . /code
WORKDIR /code

RUN pip install -r requirements.txt

ADD EntryPoint.sh /usr/local/bin/EntryPoint.sh
RUN chmod +x /usr/local/bin/EntryPoint.sh

CMD ["/usr/local/bin/EntryPoint.sh"]
