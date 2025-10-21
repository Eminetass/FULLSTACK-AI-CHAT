from fastapi import FastAPI
from pydantic import BaseModel
from transformers import pipeline

app = FastAPI(title="AI Sentiment Service")

# Hugging Face pipeline (CPU ile çalışır)
clf = pipeline("sentiment-analysis", model="distilbert-base-uncased-finetuned-sst-2-english")

class Req(BaseModel):
    text: str

@app.post("/analyze")
def analyze(req: Req):
    res = clf(req.text)[0]  # örn: {'label': 'POSITIVE', 'score': 0.99}
    return {"label": res["label"].lower(), "score": float(res["score"])}

@app.get("/health")
def health():
    return {"ok": True}