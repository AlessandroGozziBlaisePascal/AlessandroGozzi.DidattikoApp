# DidattikoApp - E-Commerce Libri Scolastici Usati

Applicazione desktop per la compravendita di libri di testo scolastici usati, sviluppata con architettura pulita (DDD / Clean Architecture) in C# e WPF.

## 🚀 Tecnologie Utilizzate

* **Linguaggio:** C# (.NET)
* **GUI / Interfaccia Grafica:** WPF (Windows Presentation Foundation)
* **Database & ORM:** SQLite / Entity Framework Core (gestione migrazioni)
* **Testing:** xUnit / FluentAssertions

## 🏗️ Architettura del Progetto

Il progetto è strutturato seguendo i principi di **Domain-Driven Design (DDD)** e **Clean Architecture**:

* `Domain`: Entità di dominio, Value Objects, Aggregate Roots ed Eventi (regole di business fondamentali).
* `Application`: CQRS / Use Cases (gestione comandi, query, contratti di servizio e gestori eventi).
* `Infrastructure`: Implementazione della persistenza dei dati (DB), repository, servizi esterni e autenticazione.
* `SharedKernel`: Componenti e contratti condivisi tra i diversi livelli dell'applicazione.
* `WPF`: Interfaccia utente grafica dell'applicazione (Views, ViewModels e asset di interfaccia).
* `DomainTests`: Unit test dedicati alla validazione del livello di dominio e delle regole di business.

## 🛠️ Funzionalità Principali

* **Autenticazione e Profilo:** Login, registrazione e gestione token per le sessioni utente.
* **Gestione Catalogo & Ordini:** Ricerca e gestione di libri scolastici usati, carrello e spedizioni.
* **Portafoglio Digitale:** Gestione metodi di pagamento (Wallet/Carte/Coordinate) e calcolo costi di spedizione.

## ⚙️ Come eseguire il progetto

1. Clonare il repository:
   ```bash
   git clone [https://github.com/AlessandroGozzi/DidattikoApp.git](https://github.com/AlessandroGozzi/DidattikoApp.git)
