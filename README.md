# 💳 PayByLink API

Backend SaaS for generating payment links (Stripe Checkout) with automated delivery via WhatsApp and SMS (Twilio).

Designed to demonstrate real-world payment integrations, event-driven processing, and secure multi-tenant backend architecture.

---

## 🚀 What This Project Demonstrates

- Real integration with Stripe (Checkout + Webhooks)
- Asynchronous payment confirmation handling
- Twilio integration (WhatsApp + SMS fallback logic)
- JWT authentication with refresh token strategy
- Secure password hashing (BCrypt)
- Transactional persistence with EF Core + PostgreSQL
- Operational logging of notification attempts
- SaaS-ready multi-user structure

---

## 🧠 Architecture Overview

```text
Client
  │
  ▼
ASP.NET Core API (.NET 8)
  ├── Stripe (Payments + Webhooks)
  ├── Twilio (Messaging)
  ▼
PostgreSQL

Key architectural decisions:
Externalized payment processing (no card storage)
Webhook signature validation
Transaction-safe status updates
Intelligent notification fallback (WhatsApp → SMS)
Clear separation of concerns

🔁 Payment Flow
User authenticates via JWT.
Creates a payment request.
API generates Stripe Checkout session.
Payment link sent via WhatsApp (SMS fallback).
Stripe sends checkout.session.completed.
Payment status updated to Paid.

🛠 Tech Stack
.NET 8 (ASP.NET Core Web API)
Entity Framework Core
PostgreSQL
Stripe API
Twilio API
JWT Authentication
BCrypt

🎯 Purpose
This project showcases the ability to:
Design production-oriented backend systems
Integrate third-party financial APIs
Handle event-driven workflows
Implement secure authentication patterns
Build scalable SaaS foundations
