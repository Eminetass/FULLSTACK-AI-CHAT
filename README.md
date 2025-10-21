
# FullStack + AI Chat Uygulaması

Kullanıcıların web ve mobil arayüzden mesajlaşabildiği, her mesajın anlık olarak bir AI servisi tarafından duygu analizine tabi tutulup sonuçlarının ekranda gösterildiği uçtan uca bir uygulama.

- Web: React + Vite
- Backend: .NET Core + SQLite (ChatService + IdentityService)
- AI Servis: Python + FastAPI + Gradio (Hugging Face Spaces)
- Mobil: React Native (opsiyonel, klasör hazır)

Proje mimarisi tasarlanırken **Domain-Driven Design (DDD)** ve **Onion Architecture** prensiplerinden faydalanılmış; katmanlar arası bağımlılıklar minimize edilerek sürdürülebilir, test edilebilir bir yapı hedeflenmiştir.

![Uygulama Ekranı](chat.png)

---

## İçindekiler

- [Demo Linkleri](#demo-linkleri)
- [Klasör Yapısı](#klasör-yapısı)
- [Hızlı Başlangıç (Lokal)](#hızlı-başlangıç-lokal)
- [AI Servisi (Hugging Face Spaces)](#ai-servisi-hugging-face-spaces)
- [Backend (Render) Deploy](#backend-render-deploy)
- [Frontend (Vercel) Deploy](#frontend-vercel-deploy)
- [Mimari ve Akış](#mimari-ve-akış)
- [API Yüzeyi](#api-yüzeyi)
- [Geliştirme Notları](#geliştirme-notları)
- [Sık Görülen Hatalar ve Çözümler](#sık-görülen-hatalar-ve-çözümler)
- [Katkı ve Lisans](#katkı-ve-lisans)

---

## Demo Linkleri

> Aşağıdaki yer tutucuları kendi linklerinle değiştir.

- Web (Vercel): https://<senin-projen>.vercel.app
- Backend (Render): https://<senin-servisin>.onrender.com
- AI Demo UI (Gradio): https://emnts-sentimen-ai.hf.space/ui
- AI API (POST): https://emnts-sentimen-ai.hf.space/analyze
- AI Health: https://emnts-sentimen-ai.hf.space/healthz

---

## Klasör Yapısı

Bu repo yapısı, PDF’teki gereksinimleri karşılayacak şekilde ve mevcut proje durumuna uygun olarak düzenlenmiştir.
FULLSTACK-AI-CHAT/

├─ ai-service/ # Python + FastAPI + Gradio (Hugging Face Spaces)

│ ├─ app.py

│ ├─ requirements.txt

│ └─ Dockerfile # (opsiyonel; Spaces ayarına göre)
│

├    ─ frontend/ # React + Vite (Web arayüzü)

│    ├─ src/
│    
├    ─ public/

│    ├─ index.html

│    ├─ package.json

│    ├─ vite.config.ts

│    └─ .env.example
│
├─    mobile/ # React Native (opsiyonel; ileride entegre için hazır)
│    
      └─ src/
│
├     ─ services/ # .NET Core servisleri (Onion/DDD yaklaşımı)
│ ├─ ChatService/ # Mesajlaşma, sohbet ve AI entegrasyonu
│ └─ IdentityService/ # Kullanıcı (rumuz) kimlik işlemleri, JWT
│
├─ .vscode/
├─ .venv/ # (lokal sanal ortam; repoya dahil edilmez)
├─ .gitignore
├─ package.json # root yardımcı scriptler için (varsa)
├─ package-lock.json
├─ README.md
└─ chat.png # README görseli


> Not: .NET servis proje içeriğinde tipik olarak Controllers, Application, Domain, Infrastructure gibi katmanlar ve konfigürasyon dosyaları bulunur. Proje, Onion/DDD prensiplerine göre düzenlenmiştir.

---

## Hızlı Başlangıç (Lokal)

Önkoşullar:
- Node.js (>=18), npm
- .NET SDK (>=8.0)
- Python 3.10+
- Git
- (Mobil için) Android Studio + SDK

### 1) AI Servisi (lokal çalıştırma opsiyonel)
AI servisimiz Hugging Face Spaces’te host ediliyor; lokal çalıştırmaya gerek yok. Test için doğrudan Space API’sini kullanabilirsiniz:
```bash
curl -X POST https://emnts-sentimen-ai.hf.space/analyze \
  -H "Content-Type: application/json" \
  -d '{"text":"bugün harika hissediyorum"}'
2) Backend (ChatService + IdentityService)
Aşağıdaki komutları her servis klasöründe uygulayın.

bash
Copy
cd services/IdentityService
dotnet restore
dotnet build
dotnet run

# Yeni bir terminal
cd services/ChatService
dotnet restore
dotnet build
dotnet run
Gerekli ortam değişkenleri (örnek):

bash
Copy
# Windows PowerShell örneği
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ConnectionStrings__DefaultConnection="Data Source=app.db"
$env:JWT__Issuer="fullstack-ai-chat"
$env:JWT__Audience="fullstack-ai-chat"
$env:JWT__Secret="en-az-32-40-uzunlukta-guclu-bir-secret-anahtari"
$env:AI_SERVICE_URL="https://emnts-sentimen-ai.hf.space/analyze"
Önemli: JWT Secret yeterince uzun ve güçlü olmalıdır. IdentityService ile ChatService’deki JWT ayarları birebir aynı olmalıdır.

3) Frontend (React + Vite)
bash
Copy
cd frontend
npm install
.env dosyası oluşturun:

bash
Copy
# frontend/.env
VITE_API_BASE_URL=https://localhost:5001   # Render URL’inle değiştir
Geliştirme sunucusu:

bash
Copy
npm run dev
Tarayıcı: http://localhost:5173

AI Servisi (Hugging Face Spaces)
Model/önbellek izin hatalarını önlemek için app.py başında:
python
Copy
import os
os.environ["HF_HOME"] = "/tmp"
os.environ["TRANSFORMERS_CACHE"] = "/tmp"
Gradio flagging kapalı olmalı (yazma izin hataları için):
python
Copy
demo = gr.Interface(
    fn=gr_predict,
    inputs=gr.Textbox(lines=3, label="Message"),
    outputs=[gr.Label(num_top_classes=3, label="Sentiment"), gr.Number(label="Score")],
    title="Sentiment Analysis",
    description="Simple API + UI. POST /analyze for JSON.",
    allow_flagging="never"  # veya flagging_options=None
)
requirements.txt (öneri):
fastapi>=0.115
uvicorn[standard]>=0.30
gradio>=4.44.1
transformers>=4.44
torch --extra-index-url https://download.pytorch.org/whl/cpu
Canlı linkler:
UI: https://emnts-sentimen-ai.hf.space/ui
API: https://emnts-sentimen-ai.hf.space/analyze
Health: https://emnts-sentimen-ai.hf.space/healthz
Backend (Render) Deploy
Render Dashboard → New → Web Service:

Name: fullstack-ai-chat-backend
Root Directory: services/ChatService (IdentityService’i ayrı servis olarak isterseniz ayrıca deploy edin)
Build Command:
dotnet build
Start Command:
dotnet run --urls=http://0.0.0.0:10000
Environment Variables:
ASPNETCORE_URLS = http://0.0.0.0:10000
ConnectionStrings__DefaultConnection = Data Source=app.db
JWT__Issuer = fullstack-ai-chat
JWT__Audience = fullstack-ai-chat
JWT__Secret = en-az-32-40-uzunlukta-guclu-bir-secret-anahtari
AI_SERVICE_URL = https://emnts-sentimen-ai.hf.space/analyze
Deploy tamamlanınca örnek URL:

https://<senin-servisin>.onrender.com
Not: Render Free instance’lar uykuya geçebilir; ilk istek yavaş gelebilir.

Frontend (Vercel) Deploy
Vercel → New Project → GitHub repo → Root Directory = frontend

Framework: React
Environment Variables:
VITE_API_BASE_URL = https://<senin-servisin>.onrender.com
Deploy bittiğinde örnek URL:
https://<senin-projen>.vercel.app
Mimari ve Akış
Proje mimarisi tasarlanırken DDD ve Onion Architecture prensiplerinden faydalanılmıştır. Temel akış:

Kullanıcı web arayüzünden mesaj gönderir.
Frontend → Backend (ChatService) API’sine istek atar.
Backend mesajı DB’ye yazar ve AI servisine (Hugging Face Spaces) analiz için gönderir.
AI sonucu backend’e döner; backend hem veritabanına kaydeder hem de frontend’e döner.
Frontend sonuçları gerçek zamanlı listede gösterir.
API Yüzeyi
Örnek uçlar; kendi servislerinize göre güncelleyin.

Auth (IdentityService)
POST /api/auth/register → rumuzla kayıt
POST /api/auth/login → JWT al
Chat (ChatService)
GET /api/messages → Mesaj listesi
POST /api/messages → Yeni mesaj + AI analizi tetikler
GET /healthz → Sağlık kontrolü
Örnek istek:

bash
Copy
curl -X POST https://<senin-servisin>.onrender.com/api/messages \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT>" \
  -d '{"text":"bugün harika hissediyorum"}'
Örnek yanıt:

json
Copy
{
  "id": 123,
  "text": "bugün harika hissediyorum",
  "sentiment": "pozitif",
  "score": 0.97,
  "createdAt": "2025-10-21T12:34:56Z"
}
Geliştirme Notları
JWT Secret uzun ve güçlü olmalıdır; iki .NET servisinde aynı olmalıdır.
Vite kullandığımız için web tarafında ortam değişkenleri VITE_ ile başlar.
AI servisi izin hataları:
HF_HOME ve TRANSFORMERS_CACHE → /tmp
Gradio’da allow_flagging="never"
Sık Görülen Hatalar ve Çözümler
Hugging Face /data PermissionError:
app.py başına:
python
Copy
os.environ["HF_HOME"] = "/tmp"
os.environ["TRANSFORMERS_CACHE"] = "/tmp"
Gradio flagging yazma hatası:
allow_flagging="never" veya flagging_options=None
401/403 JWT:
Issuer/Audience/Secret tüm servislerde aynı olmalı; token süresini kontrol edin.
Vite env değişkeni görülmüyor:
Değişken adını VITE_ ile başlatın ve dev server’ı yeniden başlatın.
Katkı ve Lisans
PR’lar açıktır.
Lisans: MIT












