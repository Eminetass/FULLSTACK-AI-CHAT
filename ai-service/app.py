import os
os.environ["HF_HOME"] = "/tmp"
os.environ["TRANSFORMERS_CACHE"] = "/tmp"
from typing import Literal
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from transformers import pipeline
import gradio as gr

# ---- API Şemaları ----
class AnalyzeRequest(BaseModel):
    text: str

class AnalyzeResponse(BaseModel):
    label: Literal["POSITIVE", "NEUTRAL", "NEGATIVE"]
    score: float

# ---- Uygulama ----
app = FastAPI(title="Sentiment AI Service", version="1.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], allow_credentials=True,
    allow_methods=["*"], allow_headers=["*"]
)

MODEL_ID = os.getenv("MODEL_ID", "distilbert-base-uncased-finetuned-sst-2-english")
_THRESHOLD_NEUTRAL = float(os.getenv("NEUTRAL_THRESHOLD", "0.6"))

_pipe = None
def get_pipe():
    global _pipe
    if _pipe is None:
        _pipe = pipeline("sentiment-analysis", model=MODEL_ID)
    return _pipe

@app.on_event("startup")
def _warmup():
    p = get_pipe()
    _ = p("Warm up for cold start")

@app.get("/healthz")
def healthz():
    return {"status": "ok", "model": MODEL_ID}

@app.get("/")
def root():
    return {"message": "Use POST /analyze for JSON or open /ui for demo"}

@app.post("/analyze", response_model=AnalyzeResponse)
def analyze(req: AnalyzeRequest):
    p = get_pipe()
    res = p(req.text)[0]  # {'label': 'POSITIVE', 'score': 0.99}
    label = res["label"].upper()
    score = float(res["score"])
    # 2 sınıflı modelden 'NEUTRAL' türet
    if score < _THRESHOLD_NEUTRAL:
        label = "NEUTRAL"
    elif label not in {"POSITIVE", "NEGATIVE"}:
        # farklı modellerde label adları için güvenliğe al
        label = "POSITIVE" if "POS" in label else "NEGATIVE"
    return {"label": label, "score": round(score, 4)}

# ---- Gradio UI ----
def gr_predict(text: str):
    out = analyze(AnalyzeRequest(text=text))
    return {out.label: out.score}, out.score

demo = gr.Interface(
    fn=gr_predict,
    inputs=gr.Textbox(lines=3, label="Message"),
    outputs=[
        gr.Label(num_top_classes=3, label="Sentiment"),
        gr.Number(label="Score")
    ],
    title="Sentiment Analysis",
    description="Simple API + UI. POST /analyze for JSON.",
    allow_flagging="never"
)

# UI'yi /ui altına mount et
app = gr.mount_gradio_app(app, demo, path="/ui")