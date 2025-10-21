FROM python:3.10-slim

ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1 \
    PIP_NO_CACHE_DIR=1 \
    HF_HOME=/data \
    TRANSFORMERS_CACHE=/data

WORKDIR /app

# temel araçlar
RUN apt-get update && apt-get install -y --no-install-recommends git && rm -rf /var/lib/apt/lists/*

COPY requirements.txt /app/
RUN pip install -U pip \
 && pip install -r requirements.txt \
 && pip install --index-url https://download.pytorch.org/whl/cpu torch

COPY app.py /app/

EXPOSE 7860
CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "7860", "--workers", "1"]