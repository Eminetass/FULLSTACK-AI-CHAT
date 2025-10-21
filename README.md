# FULLSTACK + AI CHAT UYGULAMASI

Kullanıcıların web arayüzünden (ve opsiyonel olarak mobilden) mesaj gönderebildiği; her mesajın anlık olarak bir AI servisi tarafından duygu analizine tabi tutulup (pozitif / nötr / negatif) sonuçların ekranda gösterildiği uçtan uca bir uygulama.

- Web: React + Vite (TypeScript)
- Backend: .NET Core + SQLite (Microservices: IdentityService + ChatService)
- AI Servis: Python + FastAPI + Gradio (Hugging Face Spaces)
- Mimari: Domain-Driven Design (DDD), Onion Architecture, SOLID

> Proje mimarisi tasarlanırken DDD ve Onion Architecture prensiplerinden faydalanılmıştır. Katmanlar arası bağımlılıklar minimize edilerek test edilebilir ve sürdürülebilir bir yapı hedeflenmiştir.

![Uygulama Ekranı](chat.png)

---

## İçindekiler

- [Demo Linkleri](#demo-linkleri)
- [Klasör Yapısı](#klasör-yapısı)
- [Ön Gereksinimler](#ön-gereksinimler)
- [Hızlı Başlangıç (Lokal Kurulum)](#hızlı-başlangıç-lokal-kurulum)
  - [AI Servisi (Uzak—HF Spaces)](#ai-servisi-uzakhf-spaces)
  - [Backend Servisleri (Identity + Chat)](#backend-servisleri-identity--chat)
  - [Web (React + Vite)](#web-react--vite)
- [Ortam Değişkenleri](#ortam-değişkenleri)
- [Dağıtım (Deploy) Rehberi](#dağıtım-deploy-rehberi)
  - [Hugging Face Spaces (AI Servis)](#hugging-face-spaces-ai-servis)
  - [Render (ChatService / .NET Core + SQLite)](#render-chatservice--net-core--sqlite)
  - [Vercel (React Web)](#vercel-react-web)
- [API Dokümantasyonu](#api-dokümantasyonu)
  - [IdentityService (Auth)](#identityservice-auth)
  - [ChatService (Mesajlar + AI Entegrasyonu)](#chatservice-mesajlar--ai-entegrasyonu)
- [Mimari Notlar (DDD + Onion)](#mimari-notlar-ddd--onion)
- [Sık Görülen Hatalar ve Çözümleri](#sık-görülen-hatalar-ve-çözümleri)
- [Katkı, Lisans](#katkı-lisans)

---

## Demo Linkleri

> Aşağıdaki yer tutucu alanları kendi canlı linklerinle değiştir.

- Web (Vercel): https://<senin-projen>.vercel.app
- Backend (Render / ChatService): https://<senin-servisin>.onrender.com
- AI Demo UI (Gradio): https://emnts-sentimen-ai.hf.space/ui
- AI API (POST): https://emnts-sentimen-ai.hf.space/analyze
- AI Health: https://emnts-sentimen-ai.hf.space/healthz

---

## Klasör Yapısı

Bu yapı doğrudan repodaki mevcut dosya/klasör düzeni ile uyumludur.
FULLSTACK-AI-CHAT/


├─ ai-service/ # Python + FastAPI + Gradio (Hugging Face Spaces)
│ ├─ app.py
│ ├─ requirements.txt
│ ├─ Dockerfile
│ └─ ai-sentiment/ # (model/ek kodlar için - opsiyonel)


│
├─ frontend/ # React + Vite (TypeScript)
│ ├─ public/
│ ├─ src/
│ ├─ index.html
│ ├─ package.json
│ ├─ package-lock.json
│ ├─ vite.config.ts
│ ├─ tsconfig.json
│ ├─ tsconfig.node.json
│ ├─ eslint.config.js
│ └─ .env # (lokalde; repoya eklemeyin)
│
├─ mobile/ # React Native (opsiyonel - hazırlık)
│ └─ src/


│
├─ services/ # .NET Core servisleri (Microservices + DDD/Onion)


│ ├─ ChatService/
│ │ ├─ Controllers/
│ │ ├─ bin/
│ │ ├─ obj/
│ │ └─ ... (Program.cs, appsettings*.json, .csproj)
│ │


│ └─ IdentityService/
│ ├─ Controllers/
│ ├─ bin/
│ ├─ obj/
│ └─ ... (Program.cs, appsettings.json, *.csproj)
│


├─ README.md
├─ package.json # (kök yardımcı scriptler - varsa)
├─ package-lock.json
├─ .gitignore


Notlar:
- `node_modules`, `.venv`, `bin`, `obj` gibi dizinler gitignore kapsamındadır.
- Backend iki ayrı servis olarak tutulur: `IdentityService` (JWT auth) ve `ChatService` (mesaj + AI entegrasyonu).
- AI servisi Hugging Face Spaces için `app.py` + `requirements.txt` + (gerekirse) `Dockerfile` içerir.

---



## Ön Gereksinimler

- Git
- Node.js 18+ ve npm
- .NET SDK 8.0+
- Python 3.10+
- (Opsiyonel) Android Studio + SDK (mobil geliştirme için)

---

Mimari Notlar (DDD + Onion)
Domain (çekirdek):
Entity, Value Object, Domain Service’ler
İş kuralları burada; dışa bağımlılık yok
Application:
Use-case’ler (komut/sorgu), DTO’lar, portlar
Domain’i kullanarak iş akışını yönetir
Infrastructure:
Repository implementasyonları (EF Core/SQLite)
parti entegrasyonlar (AI servis çağrısı vb.)
Presentation (API):
Controller/Endpoint’ler
Validasyon ve auth kontrolü
Microservices:
IdentityService ve ChatService bağımsız deploy edilebilir
JWT üzerinden yetkilendirme; ortak ayarlar (Issuer/Audience/Secret)
SOLID:
Tek sorumluluk, arayüz ayrımı, bağımlılıkları tersine çevirme gibi prensipler benimsenmiştir.

## Hızlı Başlangıç (Lokal Kurulum)

### AI Servisi (Uzak—HF Spaces)

AI servisiniz halihazırda Hugging Face Spaces üzerinde çalışıyor. Lokal kurulum zorunlu değildir. Doğrudan Space API’sini test edebilirsiniz:

```bash
curl -X POST https://emnts-sentimen-ai.hf.space/analyze \
  -H "Content-Type: application/json" \
  -d '{"text":"bugün harika hissediyorum"}'


Örnek cevap:

json
{
  "label": "pozitif",
  "score": 0.97
}


Eğer AI servisini lokalde denemek isterseniz: cd ai-service && pip install -r requirements.txt && uvicorn app:app --host 0.0.0.0 --port 8000


Backend Servisleri (Identity + Chat)
İki servisi ayrı terminallerde çalıştırın.



IdentityService
bash
Copy
cd services/IdentityService
dotnet restore
dotnet build
dotnet run



ChatService
bash
Copy
cd services/ChatService
dotnet restore
dotnet build
dotnet run


Gereken ortam değişkenleri (Windows PowerShell örneği):

powershell
Copy
# Her iki servis için ortak/JWT uyumlu olmalı:
$env:JWT__Issuer="fullstack-ai-chat"
$env:JWT__Audience="fullstack-ai-chat"
$env:JWT__Secret="en-az-32-40-uzunlukta-guclu-bir-secret-anahtari"


# ChatService için:
$env:ConnectionStrings__DefaultConnection="Data Source=app.db"
$env:AI_SERVICE_URL="https://emnts-sentimen-ai.hf.space/analyze"
$env:ASPNETCORE_URLS="http://0.0.0.0:5001"    # lokalde portu kendine göre ayarlayabilirsin


# IdentityService için:
$env:ASPNETCORE_URLS="http://0.0.0.0:5000"
Önemli: IdentityService ve ChatService’de JWT ayarları (Issuer/Audience/Secret) birebir aynı olmalı. Secret en az 32–40 karakterlik güçlü bir anahtar olmalı.



Web (React + Vite)
bash

cd frontend
npm install
frontend/.env oluşturun:

env

# Frontend tarafında Vite değişkenleri "VITE_" ile başlamak zorundadır.
VITE_API_BASE_URL=http://localhost:5001
Geliştirme sunucusu:

bash

npm run dev
Tarayıcı: http://localhost:5173




Ortam Değişkenleri
Aşağıdaki değişkenler proje genelinde kullanılır. Dağıtım ortamlarında ilgili platformların (Render, Vercel, HF Spaces) env alanlarına ekleyin.

Ortak/JWT:
JWT__Issuer
JWT__Audience
JWT__Secret
ChatService:
ConnectionStrings__DefaultConnection → Data Source=app.db
AI_SERVICE_URL → https://emnts-sentimen-ai.hf.space/analyze
ASPNETCORE_URLS → http://0.0.0.0:10000 (Render için)
IdentityService:
ASPNETCORE_URLS → http://0.0.0.0:<port>
Frontend (Vercel/Vite):
VITE_API_BASE_URL → Backend’in public URL’i (Render linkiniz)
AI Service (HF Spaces içi):
HF_HOME=/tmp
TRANSFORMERS_CACHE=/tmp


API Dokümantasyonu
Aşağıdaki uçlar örnek olup, gerçek controller/route isimlerinizle birebir uyumlu olacak şekilde düzenlenmiştir.

IdentityService (Auth)
POST /api/auth/register
Body:
json
Copy
{
  "username": "emine"
}
Response:
json
Copy
{
  "userId": "xxxx",
  "username": "emine"
}
POST /api/auth/login
Body:
json
Copy
{
  "username": "emine"
}
Response:
json
Copy
{
  "token": "<JWT>"
}
Not: Basitlik için parola zorunluluğu olmayabilir. Üretim ortamlarında parola/policy eklenmelidir.

ChatService (Mesajlar + AI Entegrasyonu)
GET /api/messages
Headers: Authorization: Bearer <JWT>
Response (örnek):
json
Copy
[
  {
    "id": 1,
    "text": "bugün harika hissediyorum",
    "sentiment": "pozitif",
    "score": 0.97,
    "createdAt": "2025-10-21T12:34:56Z",
    "username": "emine"
  }
]
POST /api/messages
Headers: Authorization: Bearer <JWT>
Body:
json
Copy
{
  "text": "bugün harika hissediyorum"
}
Akış:
Mesaj DB’ye yazılır.
AI_SERVICE_URL’e (HF Spaces) { "text": "..." } POST edilir.
Dönüşte alınan label/score mesaj kaydına eklenir ve yanıtlanır.
Response (örnek):
json
Copy
{
  "id": 2,
  "text": "bugün harika hissediyorum",
  "sentiment": "pozitif",
  "score": 0.97,
  "createdAt": "2025-10-21T12:35:10Z",
  "username": "emine"
}
cURL örneği:

bash
Copy
curl -X POST https://<senin-servisin>.onrender.com/api/messages \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT>" \
  -d '{"text":"bugün harika hissediyorum"}'



Sık Görülen Hatalar ve Çözümleri
HF Spaces “PermissionError: /data”:
app.py başında:
python
Copy
os.environ["HF_HOME"] = "/tmp"
os.environ["TRANSFORMERS_CACHE"] = "/tmp"
HF Spaces Gradio “flagging” yazma hatası:
allow_flagging="never" veya flagging_options=None
401/403 (JWT):
Issuer/Audience/Secret tüm servislerde aynı mı?
Token süresi ve clock skew kontrolü
CORS hataları:
ChatService/IdentityService için uygun CORS politikası ekleyin
Vite env okunmuyor:
Değişken adları VITE_ ile başlamalı
Sunucuyu yeniden başlatın
Render “yavaş ilk yanıt”:
Free instance uyku/soğuk başlatma; ilk istek uzun sürebilir

