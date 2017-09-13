FROM python:3.6.1
ENV PYTHONUNBUFFERED 1

RUN mkdir /code
ADD . /code
WORKDIR /code

RUN pip install -r requirements.txt

CMD["python", "./worker.py"]
