
## 1. PUBLIC FRONT-OFFICE

### 1.1 Top Navigation Bar (all public pages)

**Left:** LIMTIC logo (shield + text) + "Laboratoire LIMTIC" wordmark  
**Center nav links:** Accueil · Chercheurs · Publications · Événements · Axes de Recherche · Contact  
**Right:** "Se connecter" outlined button (→ /login) + language toggle FR/EN  
**Behavior:** Transparent on hero, white + shadow on scroll. Mobile: hamburger → slide-over drawer. Active link has accent underline.

---

### 1.2 Home Page (`/`)

#### Hero Section
- Full-width, 100vh, gradient background (`#0F2557` → `#1E40AF` → `#0D9488`)
- Animated particle/grid network SVG overlay (subtle, not distracting)
- Center content:
  - ISI + LIMTIC dual logo row
  - H1: "Bienvenue au Laboratoire LIMTIC"
  - Subtitle: "Laboratoire d'Informatique, Modélisation et Traitement de l'Information et de la Connaissance"
  - 3 CTA buttons: "Nos Publications" (filled white) · "Notre Équipe" (outlined white) · "Nous Contacter" (ghost white)
- Bottom: animated scroll indicator

#### Statistics Bar (full-width dark band below hero)
4 animated counters in a row, white text on `#0F2557`:
- 🔬 N Chercheurs permanents
- 📄 N Publications scientifiques
- 🎓 N Doctorants en cours
- 📅 N Événements organisés

#### Axes de Recherche Section (gray background)
- H2: "Nos Axes de Recherche"
- 3-column card grid, each card:
  - Large icon (custom SVG per axis) + colored accent top border
  - Axis title (bold) + short description
  - Colored thematic chips (3 max shown + "+N more")
  - Responsible researcher name with avatar thumbnail
  - "Voir l'axe →" link

#### Recent Publications Section (white background)
- H2 + "Voir toutes →" link
- Horizontal scroll of 3 publication cards, each showing:
  - Type badge (Q1 green · CORE A* gold · etc.)
  - Year chip
  - Title (2 lines max, ellipsis)
  - Authors (comma-separated, first 3 + "et al.")
  - Journal/conference name italic
  - DOI link (external icon)
  - "Lire →" button

#### Upcoming Events Widget ("Agenda")
- Timeline/list layout, left accent border colored by event type
- Each item: date block (day big + month) · event type chip · title · location pin icon · "Voir →"
- Max 3 upcoming, "Voir tous les événements →" below

#### News / Latest Activity Feed
- 2-column masonry or list, each item:
  - Icon (publication / event / member) + timestamp relative ("Il y a 3j")
  - Short description
  - Link

#### Footer
- Dark background (`#0F172A`)
- 4 columns: Logo + mission statement · Quick links · Contact info · Social links (LinkedIn, ResearchGate, ORCID, Twitter/X)
- Bottom bar: © year · Legal · Privacy · Back to top button

---

### 1.3 Team Page (`/equipe`)

#### Page Header
- Gradient banner (smaller, 300px) with title "Notre Équipe" + subtitle + breadcrumb

#### Filter Bar
- Sticky top: search input · filter by role (Chercheurs/Doctorants/Mastériens) · filter by axis · filter by status (Actif/Retraité/En cours/Soutenu)

#### Tab switcher: Chercheurs · Doctorants · Mastériens · Axes de Recherche

**Chercheurs tab:**
- 3-column responsive card grid
- Each card: photo (square, rounded-xl, object-cover) · name bold · grade (colored) · speciality chip · axis chip · email link · ORCID/Scholar icons row · "Voir profil →"

**Doctorants tab:**
- 3-column grid
- Each card: photo thumbnail · name · thesis title (2 lines) · director name (link) · year enrolled · status pill (En cours / Soutenu + date)

**Mastériens tab:**
- 3-column grid
- Each card: photo · name · memoir subject · supervisor · academic year · status pill

**Axes de Recherche tab:**
- Full-width cards, each:
  - Left accent colored stripe
  - Axis title H3 + responsible name
  - Description paragraph
  - Thematic chips row
  - Member avatar stack (max 5 + "+N")
  - "Voir les publications de cet axe →"

---

### 1.4 Individual Researcher Page (`/chercheurs/:id`)

**Layout:** 3-column left sidebar + main content (sticky sidebar on scroll)

**Left sidebar (sticky):**
- Large photo (200px circle with border)
- Name H2 · Grade · Axis badge
- Email · Office · Phone (with icons)
- External profile buttons: ORCID · Google Scholar · ResearchGate · LinkedIn (each with platform icon)
- "Modifier mon profil" button (visible only to the researcher themselves when logged in)

**Main content — 4 tabs:**

*Tab 1: Biographie*
- WYSIWYG rich text biography
- Welcome message from director (if this is the director's page)

*Tab 2: Publications (Google Scholar style)*
- Sortable by year desc (default), type, citations
- Filter: type dropdown · year dropdown · keyword search
- Each entry row:
  - Type badge (colored: Q1 green / CORE A* gold / CORE A blue / B gray / C light)
  - Title (bold, clickable → opens publication detail modal)
  - Authors (researcher name **bold**, others normal)
  - Venue: journal or conference name, italic, volume/issue/pages
  - Year · DOI link · PDF download icon
  - Citations count chip
  - Edit icon (only for the researcher themselves, or admin)
- Export row: "BibTeX" + "CSV" export buttons

*Tab 3: Encadrements*
- Two sub-sections: "Doctorants" and "Mastériens"
- Each with mini-cards: photo · name · subject · status pill · year
- "Ajouter un encadrement" button (chercheur/admin only)

*Tab 4: Axes de recherche*
- List of axes the researcher belongs to, with thematic chips

---

### 1.5 Publications Page (`/publications`)

**Header:** gradient banner + title + stats counter row (Total · Journaux · Conférences Int · Conférences Nat)

**Filter Panel (collapsible sidebar or top bar):**
- Full-text search (title, authors, keywords)
- Type: All · Article Journal · Conférence Internationale · Conférence Nationale · Chapitre d'ouvrage · Rapport Technique
- Year: All · dropdown of available years
- Classement: All · Q1 · Q2 · Q3 · Q4 · CORE A* · CORE A · CORE B · CORE C
- Axis: All · dropdown of axes
- Visibility: (logged-in users only): All · Publique · Privée

**Publications List (sorted by year desc):**
Each row/card:
- Left: year block (big number, colored background)
- Type badge (colored pill): Q1=green, Q2=teal, CORE A*=gold, CORE A=blue, B=gray, C=light
- Title H4 bold (clickable)
- Authors: internal researchers as links, external as plain text, all comma-separated
- Venue: Journal name in italics OR Conference name in italics + location + country
- Volume/issue/pages (for journals) · Classement CORE (for conferences)
- Keywords chips (max 4 + "+N more")
- Icons row: PDF download · DOI external · BibTeX export · Share

**Pagination:** server-side, 20 per page, with "Load more" option

**Export bar:** "Exporter BibTeX" + "Exporter CSV" (full filtered list)

---

### 1.6 Publication Detail Modal / Page (`/publications/:id`)

Full-page or large modal with:
- Type badge + classification badge (Q1/CORE A*/etc.)
- Title H2
- Authors with photos (avatars for internal, initials for external) — each internal author links to their profile
- **Abstract** section (full WYSIWYG rendered text)
- Details grid:
  - For journals: Journal name · Volume · Issue · Pages · DOI · IF · Quartile · SNIP · ISSN
  - For conferences: Conference name · Location · Country · Year · CORE ranking · Indexation (IEEE/ACM/Scopus chips)
  - For books: Book title · Editor · ISBN · Pages
  - For reports: Report number · Institution
- **Keywords:** chip list
- **Axes de Recherche:** linked
- **Year** + **Status** badge (Publié/Soumis/Brouillon — only shown to logged-in users)
- Action buttons: "Télécharger PDF" · "Voir DOI" · "Exporter BibTeX" · "Exporter RIS"
- Related publications section (same authors or same axis)

---

### 1.7 Events Page (`/evenements`)

**Header:** gradient banner + title

**Tab bar:** À venir · En cours · Passés

**Event cards grid (3 columns):**
- Hero photo (if available, 250px height, object-cover) or placeholder with event-type icon
- Type chip (SÉMINAIRE / ATELIER / JOURNÉE D'ÉTUDE / CONFÉRENCE) + status chip
- Title H3 bold
- Date range: icon + formatted dates (e.g., "15 juin 2026, 09h00 – 17h00")
- Location: pin icon + venue name
- Short description (3 lines, ellipsis)
- Speakers: avatar stack + "N intervenants"
- "Voir les détails →" button

---

### 1.8 Event Detail Page (`/evenements/:id`)

**Full hero image** (if photos exist, 500px parallax) or gradient placeholder with event type icon

**Layout:** main content (70%) + right sidebar (30%)

**Main content:**
- Breadcrumb: Événements → [Event title]
- Title H1 + type + status chips
- Rich text description (HTML rendered fully — paragraphs, lists, bold, etc.)
- **Programme scientifique:** Rich text or PDF embed/download button
- **Intervenants section:** card per speaker with name · affiliation · role · photo (if available) · talk title

**Photo Gallery:**
- Grid of photos (masonry: 3 cols), each clickable
- Lightbox on click: full image · caption below · prev/next arrows · keyboard nav · close (X)
- Photos are draggable to reorder (admin/superadmin only)
- "Toutes les photos (N)" toggle to expand

**Right sidebar:**
- Date(s) block (calendar style)
- Location block with embedded Google Maps iframe
- Speakers count
- "Télécharger le programme" button (if PDF exists)
- Share buttons

---

### 1.9 Contact Page (`/contact`)

**Two-column layout:**

**Left column:**
- Contact form:
  - Nom complet (required)
  - Email (required, validated)
  - Sujet (required)
  - Message (textarea, min 20 chars, required)
  - CAPTCHA widget (hCaptcha or reCAPTCHA)
  - "Envoyer le message" button (with loading state + success/error toast)

**Right column:**
- Lab address card with pin icon
- Phone card
- Email card
- Social links (LinkedIn, ResearchGate, etc.)
- Embedded Google Maps iframe (full width, 300px height)

---

## 2. AUTHENTICATION

### 2.1 Login Page (`/login`)

**Centered card layout** (no sidebar), full-page gradient background:
- LIMTIC logo + "Espace sécurisé" label
- H2: "Connexion"
- Email input with icon
- Password input with show/hide toggle
- "Mot de passe oublié ?" link → opens email reset modal
- "Se connecter" button (full-width, primary)
- Error inline alert (red border input + error message below)
- On success: redirect based on role → `/dashboard`
- No registration link (accounts created by admin only)

### 2.2 Password Reset Flow

- Page 1: Email input → "Envoyer le lien de réinitialisation" → success message
- Page 2: Token-validated form → new password + confirm → success redirect to login

---

## 3. DASHBOARD — SHARED LAYOUT (ALL AUTHENTICATED ROLES)

### 3.1 Dashboard Shell

**Persistent sidebar (left, 260px):**
- Top: LIMTIC logo + "Back-office" label
- User avatar (circle, 40px) + name + role badge (color-coded)
- "Mon profil" link
- Separator
- Navigation items (see per-role below), each with: icon (24px) + label + active state (left accent bar + colored background)
- Collapsed mode (72px): icons only + tooltip on hover
- Bottom: "Se déconnecter" with logout icon

**Top bar (64px):**
- Hamburger to toggle sidebar collapsed/expanded
- Page title (H2, dynamic)
- Breadcrumb
- Right: notifications bell (badge count) + user avatar dropdown (Mon profil / Déconnexion)

**Main content area:** scrollable, padding 32px, max-width 1200px, gray background

**Notification/Toast system:** top-right toasts for success/error/warning/info

---

## 4. SUPERADMIN DASHBOARD (`/dashboard` — role: SUPER_ADMIN)

> SuperAdmin has everything Admin has, plus system settings, user management, and audit logs. All items below are editable — nothing is read-only.

### Sidebar Navigation
- 🏠 Vue d'ensemble
- 👥 Utilisateurs (CRUD tous rôles)
- 📄 Publications (CRUD complet + valider/rejeter)
- 📅 Événements (CRUD complet)
- 👨‍🔬 Membres (Chercheurs · Doctorants · Mastériens — CRUD)
- 🔬 Axes de Recherche (CRUD)
- 📊 Statistiques
- 🔍 Journal d'audit
- ⚙️ Paramètres système

### 4.1 Vue d'ensemble
- 6 stat cards (animated counter): Total utilisateurs · Publications · Événements · Chercheurs · Doctorants · Mastériens
- "Activité récente" feed (last 10 audit log entries): each shows avatar + action verb (a créé / a modifié / a supprimé) + entity name + timestamp + IP
- "Publications en attente" quick list: 3 most recent SOUMIS items + "Valider" / "Rejeter" inline buttons
- "Événements à venir" quick list: next 3 with dates
- "Alertes" panel: warn about pending validations, upcoming events within 7 days

### 4.2 Gestion des Utilisateurs
- Search + role filter + status filter (actif/inactif) + date created sort
- Data table (columns): Avatar · Nom/Prénom · Email · Rôle (colored badge) · Statut (actif toggle switch) · Créé le · Actions
- Row actions: Edit (pencil icon → slide-over panel) · Reset password · Delete (confirm dialog)
- "Créer un utilisateur" button → full modal with:
  - Prénom · Nom · Email
  - Rôle selector (dropdown with role descriptions)
  - Mot de passe temporaire (generated or manual)
  - "Envoyer les identifiants par email" checkbox
  - Auto-create linked profile (Chercheur / Doctorant / Mastérien) based on role
- Bulk actions: select multiple → activate / deactivate / export CSV

### 4.3 Gestion des Publications (same as Admin 5.2, plus:)
- Can see ALL publications regardless of visibility/status
- Can directly set statut: BROUILLON → SOUMIS → PUBLIE or → REJETE
- Can edit any publication from any author
- Can delete any publication (with confirmation)
- Rejection: modal with "Motif du rejet" textarea → sent to author by notification

### 4.4 Gestion des Événements (same as Admin 5.4)
- Full CRUD
- Photo gallery management with drag-to-reorder
- Can set/override event status manually

### 4.5 Gestion des Membres
Three sub-tabs: Chercheurs · Doctorants · Mastériens

**Chercheurs:**
- Table: photo · Prénom Nom · Grade · Spécialité · Axe · Email · Actif toggle · Actions
- Create button → modal (all chercheur fields)
- Edit → slide-over panel with all editable fields (grade, bio, office, phone, ORCID, Scholar, ResearchGate, LinkedIn, axis assignment)
- Delete with confirmation (warn if has publications/encadrements)

**Doctorants:**
- Table: photo · Prénom Nom · Sujet de thèse (truncated) · Directeur (link) · Année inscription · Statut · Actions
- Create → modal: prénom, nom, sujet, directeurId (searchable dropdown of chercheurs), anneeInscription, statut, date soutenance (conditional), mention (conditional)
- Edit → slide-over with same fields + photo upload
- Delete with confirmation

**Mastériens:**
- Table: photo · Prénom Nom · Sujet mémoire (truncated) · Encadrant (link) · Promotion · Statut · Actions
- Create → modal: prénom, nom, sujet, encadrantId, promotion, statut, date diplôme (conditional)
- Edit → slide-over
- Import CSV button → upload modal with mapping preview + validation errors list

### 4.6 Axes de Recherche
- Cards layout (not table): each card shows axis name, responsible, member count, description, thematic chips
- Create button → modal:
  - Intitulé (required)
  - Description (WYSIWYG textarea)
  - Responsable (searchable dropdown of chercheurs)
  - Membres (multi-select dropdown of chercheurs with avatar chips)
  - Thématiques (tag input: type + Enter to add)
- Edit: same modal pre-filled
- Delete: confirmation dialog (warn about linked publications)

### 4.7 Statistiques
- 4 charts in 2×2 grid:
  - Publications par année (bar chart, last 5 years)
  - Publications par type (donut chart)
  - Répartition des membres par rôle (pie chart)
  - Événements par type (horizontal bar)
- Date range picker to filter all charts
- Export PNG button per chart

### 4.8 Journal d'Audit
- Table: Date/Heure · Utilisateur (avatar + email) · Action badge (CREATE green / UPDATE blue / DELETE red / LOGIN gray) · Ressource · ID Ressource · IP · Détails (expandable)
- Filters: action type · user · resource type · date range
- "Exporter CSV" button (filtered)
- Expandable row: shows full JSON diff (before/after for UPDATE actions)

### 4.9 Paramètres Système
Three accordion sections:

**Identité du laboratoire:**
- Nom du laboratoire
- Slogan/Sous-titre
- Logo upload (PNG/SVG, preview shown)
- Email de contact
- Téléphone
- Adresse postale (multiline)
- Google Maps embed URL
- Social links: LinkedIn · Twitter/X · ResearchGate · Facebook

**Configuration SMTP:**
- Hôte SMTP
- Port
- Email expéditeur (from address)
- Nom d'affichage de l'expéditeur
- Mot de passe SMTP (masked, toggle reveal)
- TLS/SSL toggle
- "Tester la configuration" button → sends test email + shows success/error toast

**SEO:**
- Per-page SEO: dropdown to select page (Accueil / Publications / Équipe / Événements / Contact)
- Titre · Meta description · Mots-clés
- Open Graph image upload
- "Sauvegarder" button

---

## 5. ADMIN DASHBOARD (`/dashboard` — role: ADMIN)

> Admin has everything except User management, system settings, and audit logs. All items are fully editable — no read-only views.

### Sidebar Navigation
- 🏠 Vue d'ensemble
- 📄 Publications (CRUD + valider/rejeter)
- 📅 Événements (CRUD complet)
- 👨‍🔬 Membres (Chercheurs · Doctorants · Mastériens — CRUD)
- 🔬 Axes de Recherche (CRUD)
- 📊 Statistiques

### 5.1 Vue d'ensemble
- 5 stat cards: Membres total · Publications en attente · Publications publiées · Événements à venir · Axes de recherche
- "Publications à valider" quick list (SOUMIS items): card per item with author + title + type + "Valider" / "Rejeter" buttons + "Voir détails" link
- "Événements à venir" timeline widget
- Recent activity feed

### 5.2 Gestion des Publications
**Full CRUD — Admin can create, edit, publish, reject, delete any publication.**

**List view:**
- Filter bar: search · type · year · statut · visibilité · axe · auteur
- Table columns: # · Type badge · Titre (truncated, clickable) · Auteurs · Annee · Statut badge · Visibilité badge · Actions
- Row actions: View (eye) · Edit (pencil) · Change status dropdown · Delete

**Statut badge colors:**
- BROUILLON: gray pill
- SOUMIS: amber/orange pill with animation pulse
- PUBLIE: green pill
- REJETE: red pill

**Create/Edit Publication — Multi-step form (modal or full page):**

Step 1 — Type & Info générale:
- Type selector (large radio cards with icon per type):
  - 📰 Article de Journal · 🌍 Conférence Internationale · 🏛 Conférence Nationale · 📚 Chapitre d'ouvrage · 📋 Rapport Technique
- Titre (required, full-width)
- Année (required, year picker)
- Résumé / Abstract (WYSIWYG textarea, required)
- Mots-clés (tag input: type + Enter)
- Visibilité toggle: PUBLIQUE / PRIVÉE
- Axes de recherche (multi-select dropdown)

Step 2 — Détails spécifiques au type:
- **If ARTICLE_JOURNAL:**
  - Nom du journal (required)
  - Volume · Numéro · Pages
  - DOI (validated format)
  - Facteur d'impact (IF) (decimal)
  - Quartile Scimago (Q1 / Q2 / Q3 / Q4 selector with color preview)
  - SNIP (decimal)
  - ISSN
  - Source classement (Scimago / Web of Science / Scopus)
- **If CONFERENCE_INT:**
  - Nom de la conférence (required)
  - Acronyme
  - Lieu · Pays
  - Année de la conférence
  - Pages
  - Classement CORE (A* / A / B / C selector with color preview: A*=gold, A=blue, B=gray, C=light)
  - Indexation (multi-select chips: IEEE / ACM / Scopus / DBLP / EI Compendex)
- **If CONFERENCE_NAT:**
  - Nom · Lieu · Année · Comité d'organisation
  - Pages
- **If CHAPITRE_OUVRAGE:**
  - Titre de l'ouvrage (required) · Éditeur · ISBN · Pages
- **If RAPPORT_TECHNIQUE:**
  - Numéro de rapport · Institution

Step 3 — Auteurs:
- Ordered list of authors (drag to reorder = sets `ordre` field)
- Each entry: toggle "Auteur interne" (→ searchable dropdown of system users) OR "Auteur externe" (→ free text input)
- "Ajouter un auteur" button (adds row)
- Remove button per row

Step 4 — Fichiers:
- PDF upload zone (drag & drop, max 20MB, shows preview filename + size, delete button)
- DOI URL (auto-linked)
- URL externe (alternative link)
- "Aperçu du PDF" button if file already uploaded

Step 5 — Validation:
- Summary card (all entered data, read-view)
- Statut selector: BROUILLON / SOUMIS / PUBLIE (admin can set directly)
- "Sauvegarder brouillon" + "Publier maintenant" buttons

**Rejection flow:**
- "Rejeter" button → confirmation modal with:
  - Publication title recap
  - "Motif du rejet" textarea (required, min 20 chars)
  - Warning: "L'auteur sera notifié par email"
  - "Confirmer le rejet" (red) + "Annuler"

### 5.3 Gestion des Membres
Same as SuperAdmin 4.5 — full CRUD for Chercheurs, Doctorants, Mastériens.
No difference in permissions.

### 5.4 Gestion des Événements

**List view:**
- Table: Type badge · Titre · Date début · Lieu · Statut badge · Photos count · Actions
- Row actions: View (eye icon, opens detail modal) · Edit · Delete

**Event status badges:**
- À VENIR: purple
- EN COURS: green (pulsing dot)
- PASSÉ: gray

**Create/Edit Event — Form:**
- Titre (required)
- Type (radio: SÉMINAIRE / ATELIER / JOURNÉE D'ÉTUDE / CONFÉRENCE)
- Date début + heure · Date fin + heure (date-time pickers)
- Lieu (required)
- Description (WYSIWYG full editor: bold/italic/lists/links/tables/embed)
- Programme scientifique: WYSIWYG editor OR file upload (PDF)
- Statut: À VENIR / EN COURS / PASSÉ (auto-computed from dates but overridable)
- Axe de recherche associé (optional dropdown)
- Intervenants section:
  - List of speakers, each: Nom · Institution · Rôle (Conférencier principal / Conférencière / Formateur / Modérateur / Organisateur · etc.) · Photo upload (optional)
  - "Ajouter un intervenant" button · Remove button per row
- Photos gallery upload:
  - Multi-file drop zone (JPEG/PNG, max 10MB each)
  - Grid preview of uploaded photos
  - Each photo: caption text input below + drag handle to reorder + delete icon
  - Drag-and-drop reorder (sortable grid)

**Event View Modal (read + actions for admin):**
- Full event details rendered (same as public detail page)
- Admin action bar at top: Edit · Manage Photos · Change Status · Delete

### 5.5 Axes de Recherche
Same as SuperAdmin 4.6 — full CRUD.

### 5.6 Statistiques
Same charts as SuperAdmin 4.7.

---

## 6. CHERCHEUR DASHBOARD (`/dashboard` — role: CHERCHEUR)

> Researchers can edit their own profile, manage their own publications, and manage their supervised students. They cannot see other users' private data.

### Sidebar Navigation
- 🏠 Vue d'ensemble
- 👤 Mon profil
- 📄 Mes publications
- 🎓 Mes encadrements
- 🔔 Notifications

### 6.1 Vue d'ensemble
- 3 stat cards: Mes publications (total) · Doctorants encadrés · Mastériens encadrés
- "Mes publications récentes" list (last 5) with statut badges
- "Mes encadrements actifs" mini-list with photos + names

### 6.2 Mon profil
**Split layout: photo sidebar (left) + form (right)**

**Photo section:**
- Large avatar (200px circle)
- "Changer la photo" button → opens image crop modal (circular crop)

**Editable form fields (all fields editable — NO disabled inputs):**
- Prénom · Nom (display only, not editable — they are set by admin on user account)
- Email (display only)
- Grade (input: Professeur / Maître de Conférences / Professeur Associé / etc.)
- Spécialité (input)
- Bureau (input)
- Téléphone (input)
- Biographie (WYSIWYG editor: bold / italic / lists / links / headings)
- ORCID (input with format hint "0000-0000-0000-0000")
- Google Scholar URL (input)
- ResearchGate URL (input)
- LinkedIn URL (input)
- Axe de recherche principal (dropdown of axes)

**"Sauvegarder les modifications" button** (full width, primary, bottom)

**Password change section** (separate card below):
- Ancien mot de passe · Nouveau mot de passe · Confirmer · "Changer le mot de passe" button

### 6.3 Mes publications
**Identical layout to Admin publications list but filtered to the researcher's own publications only.**

**Create/Edit publication (same multi-step form as Admin 5.2):**
- On submit: statut is set to SOUMIS (cannot set to PUBLIE themselves)
- Shows "En attente de validation" amber banner on SOUMIS items
- Shows "Rejeté — Motif: [motif text]" red banner on REJETE items with "Modifier et re-soumettre" button

**Actions:**
- BROUILLON: Edit · Submit for validation · Delete
- SOUMIS: View · Cancel submission (back to BROUILLON) 
- PUBLIE: View · (no edit without admin)
- REJETE: Edit · Re-submit · Delete

### 6.4 Mes encadrements
Two tabs: Doctorants · Mastériens

Each tab:
- Grid of student cards (photo · name · subject · year · status)
- "Ajouter un encadrement" button → modal:
  - **Doctorant:** Prénom · Nom · Sujet de thèse · Année d'inscription · Statut (En cours / Soutenu) · Date de soutenance (if soutenu) · Mention (if soutenu) · Photo upload
  - **Mastérien:** Prénom · Nom · Sujet de mémoire · Promotion (année académique) · Statut · Photo upload
- Edit student card → same modal pre-filled
- Delete with confirmation

### 6.5 Notifications
- List of notifications: publication validated ✅ · publication rejected ❌ · new encadrement added · system message
- Mark as read (individually or "Tout marquer comme lu")
- Empty state illustration

---

## 7. DOCTORANT DASHBOARD (`/dashboard` — role: DOCTORANT)

> Doctorants can submit their own publications and view their profile. They have read access to publications and team data.

### Sidebar Navigation
- 🏠 Vue d'ensemble
- 👤 Mon profil
- 📄 Mes publications
- 📚 Toutes les publications
- 👨‍🏫 Mon directeur
- 🔔 Notifications

### 7.1 Vue d'ensemble
- 3 stat cards: Mes publications · Publications accessibles (total visible) · Axe de recherche
- "Mon directeur de thèse" card: photo · name · grade · email · "Voir son profil →"
- "Mes publications" recent list
- "Publications récentes du labo" feed (last 5 public)

### 7.2 Mon profil
**Editable fields (all fields have inputs — NO read-only disabled fields):**
- Prénom · Nom (display info from user account)
- Sujet de thèse (textarea)
- Année d'inscription (year picker)
- Statut: En cours / Soutenu (toggle)
  - If Soutenu: date soutenance field + mention field appear
- Photo (upload + crop)
- Directeur de thèse (display — not editable by doctorant, set by admin)

"Sauvegarder" button.

### 7.3 Mes publications
**Same layout as Chercheur 6.3 — full CRUD on own publications:**
- Create → multi-step publication form (same as admin's form)
- Cannot publish themselves (statut goes to SOUMIS on submit)
- See rejection motif on rejected publications

### 7.4 Toutes les publications
- Same list view as public `/publications` page but with visibility = all (including PRIVÉE) since user is authenticated
- Read-only view (no edit actions visible)
- Export BibTeX / CSV

### 7.5 Mon directeur
- Full profile card of thesis director: photo · bio · publications list · contact

### 7.6 Notifications
Same as Chercheur 6.5.

---

## 8. MASTÉRIEN DASHBOARD (`/dashboard` — role: MASTERIEN)

> Mastériens have access to the back-office in read mode for publications, team, and axes. They can edit their own profile.

### Sidebar Navigation
- 🏠 Vue d'ensemble
- 👤 Mon profil
- 📚 Publications
- 👥 Membres de l'équipe
- 🔬 Axes de Recherche
- 👨‍🏫 Mon encadrant

### 8.1 Vue d'ensemble
- 4 stat cards: Publications accessibles · Chercheurs · Doctorants · Axes de recherche
- "Mon encadrant" mini-card
- "Publications récentes" feed

### 8.2 Mon profil
**All editable — no disabled fields:**
- Sujet de mémoire (textarea)
- Promotion (year input)
- Statut: En cours / Diplômé
- Photo upload + crop
- Encadrant (display only)

"Sauvegarder" button.

### 8.3 Publications (Read + Export)
- Same filter bar as public page
- Full detail view on click (modal — same as public detail page)
- Export BibTeX / CSV
- No create/edit/delete buttons

### 8.4 Membres de l'équipe
- Same tabs as public `/equipe` page (Chercheurs · Doctorants · Mastériens)
- Clicking a researcher opens their full profile in a modal (read-only view)
- No edit buttons

### 8.5 Axes de Recherche
- Full list with descriptions, thematic chips, member list
- Clicking "Voir les publications" filters publication list to that axis
- No edit buttons

### 8.6 Mon encadrant
- Full profile of supervisor: photo · bio · grade · email · recent publications list

---

## 9. SHARED COMPONENTS REQUIRED IN EVERY DASHBOARD

### Publication Detail View Modal
Used in all dashboards when clicking a publication title. Contains:
- All metadata fields rendered (not edit form)
- PDF viewer (embedded or download button)
- DOI link (external)
- BibTeX export button
- "Modifier" button (only visible if user has edit permission)

### Confirmation Dialog
Used before any destructive action (delete / reject / deactivate):
- Title: "Confirmer la suppression" or similar
- Description: what will be deleted + any warnings (e.g. "Cette publication a 3 auteurs associés")
- Cancel (outline) + Confirm (red filled)

### Slide-over Edit Panel
Right-side panel (400px) for quick edits without leaving the current page:
- Overlay backdrop (click to close)
- Header: entity name + "Modifier" + close button
- Scrollable form body
- Footer: "Annuler" + "Sauvegarder"

### File Upload Zone
For PDFs and images:
- Dashed border, icon + "Glisser-déposer ou cliquer pour choisir"
- Accepted formats shown
- Max size shown
- Progress bar during upload
- Uploaded file preview (thumbnail for images, filename+size+delete for PDFs)

### Rich Text Editor (WYSIWYG)
Used for: biographies, event descriptions, event programme, director welcome message.
Toolbar: Bold · Italic · Underline · H1/H2/H3 · Bulleted list · Numbered list · Link · Image embed · Quote · Table · Code block

### Notification Bell
Top-right of header:
- Badge count (red dot with number if unread)
- Dropdown panel: list of 5 most recent notifications, "Voir tout" link, "Marquer tout comme lu"

---

## 10. ENUMS & STATUS BADGES — EXACT VISUAL SPEC

### Publication Type Badges
- ARTICLE_JOURNAL: `bg:#EFF6FF text:#1D4ED8` "Journal"
- CONFERENCE_INT: `bg:#F0FDF4 text:#15803D` "Conf. Int."
- CONFERENCE_NAT: `bg:#FFF7ED text:#C2410C` "Conf. Nat."
- CHAPITRE_OUVRAGE: `bg:#FAF5FF text:#7E22CE` "Chapitre"
- RAPPORT_TECHNIQUE: `bg:#F9FAFB text:#374151` "Rapport"

### Journal Ranking Badges (Scimago Quartile)
- Q1: `bg:#059669 text:#fff` — "Q1 ★"
- Q2: `bg:#0D9488 text:#fff` — "Q2"
- Q3: `bg:#D97706 text:#fff` — "Q3"
- Q4: `bg:#9CA3AF text:#fff` — "Q4"

### Conference Ranking Badges (CORE)
- A*: `bg:#F59E0B text:#fff font-weight:700` — "CORE A*"
- A: `bg:#2563EB text:#fff` — "CORE A"
- B: `bg:#6B7280 text:#fff` — "CORE B"
- C: `bg:#D1D5DB text:#374151` — "CORE C"

### Publication Status Badges
- BROUILLON: `bg:#F1F5F9 text:#475569 border:#CBD5E1` — "Brouillon"
- SOUMIS: `bg:#FEF3C7 text:#92400E border:#FCD34D` — "Soumis" (pulsing left dot)
- PUBLIE: `bg:#D1FAE5 text:#065F46 border:#6EE7B7` — "Publié ✓"
- REJETE: `bg:#FEE2E2 text:#991B1B border:#FCA5A5` — "Rejeté ✗"

### Event Status Badges
- A_VENIR: `bg:#EDE9FE text:#4C1D95` — "À venir"
- EN_COURS: `bg:#D1FAE5 text:#065F46` with pulsing green dot — "En cours"
- PASSE: `bg:#F1F5F9 text:#475569` — "Passé"

### User Role Badges (sidebar and tables)
- SUPER_ADMIN: `bg:#1E1B4B text:#fff` "SuperAdmin 👑"
- ADMIN: `bg:#1E40AF text:#fff` "Admin"
- CHERCHEUR: `bg:#065F46 text:#fff` "Chercheur"
- DOCTORANT: `bg:#92400E text:#fff` "Doctorant"
- MASTERIEN: `bg:#374151 text:#fff` "Mastérien"

---

## 11. RESPONSIVE BREAKPOINTS

- **Mobile** (< 640px): Single column · hamburger nav · stacked cards · bottom nav for dashboard
- **Tablet** (640–1024px): Sidebar collapsed (icons only) · 2-column grids · 
- **Desktop** (> 1024px): Full sidebar expanded · 3-column grids · full tables

---

## 12. EMPTY STATES & LOADING STATES

Every list or data section must have:

**Empty state:** illustration (simple SVG, not generic) + title + description + primary CTA button  
Examples: "Aucune publication pour le moment" → "Créer ma première publication" button  
"Aucun événement à venir" → "Créer un événement" button  

**Loading skeleton:** gray animated pulse blocks matching the shape of cards/rows  

**Error state:** illustration + "Une erreur est survenue" + "Réessayer" button  

---

## 13. COMPLETE PAGE LIST (ALL SCREENS TO DESIGN)

**Public front-office (9 screens):**
1. Accueil `/`
2. Équipe `/equipe`
3. Profil chercheur `/chercheurs/:id`
4. Publications `/publications`
5. Détail publication `/publications/:id` (modal or page)
6. Événements `/evenements`
7. Détail événement `/evenements/:id`
8. Contact `/contact`
9. Login `/login` + Password Reset

**SuperAdmin dashboard (9 screens):**
10. Vue d'ensemble
11. Utilisateurs (list + create/edit modal)
12. Publications (list + multi-step form)
13. Événements (list + create/edit form)
14. Membres → Chercheurs (list + form)
15. Membres → Doctorants (list + form)
16. Membres → Mastériens (list + form)
17. Axes de Recherche (list + form)
18. Journal d'audit
19. Paramètres système (3 sections)
20. Statistiques

**Admin dashboard (8 screens, same layout, no settings/users/audit):**
21–28: Same as SuperAdmin 12–17 + Statistiques

**Chercheur dashboard (5 screens):**
29. Vue d'ensemble
30. Mon profil
31. Mes publications (list + form)
32. Mes encadrements
33. Notifications

**Doctorant dashboard (6 screens):**
34. Vue d'ensemble
35. Mon profil
36. Mes publications
37. Toutes les publications (read-only)
38. Mon directeur
39. Notifications

**Mastérien dashboard (6 screens):**
40. Vue d'ensemble
41. Mon profil
42. Publications (read)
43. Membres (read)
44. Axes de Recherche (read)
45. Mon encadrant

**Total: 45 screens minimum.**

---

## 14. KEY DESIGN RULES — STRICTLY ENFORCED

1. **No read-only disabled fields.** If a user can see a form field, they must be able to edit it. If they cannot edit it, display it as styled text (label + value), not as a disabled `<input>`. Never use grayed-out disabled inputs.
2. **No placeholder data visible in production view.** All mock content shown in designs must use realistic academic-looking data (French names, realistic publication titles, etc.)
3. **SuperAdmin and Admin share identical publication, event, member, and axis management screens.** The only difference is SuperAdmin also has the Users, Audit, and Settings screens in their sidebar.
4. **Publication multi-step form is used by ALL roles that can create publications** (Chercheur, Doctorant, Admin, SuperAdmin). The form is identical — only the available target statut differs.
5. **Event photo gallery** must include lightbox with full-image view, captions, prev/next navigation, and keyboard support on every screen it appears.
6. **Every list that has more than 10 items** must have server-side pagination (page numbers + prev/next). 
7. **All tables** must support column sorting (clickable headers with asc/desc indicator).
8. **Import CSV** buttons exist on Chercheurs, Doctorants, and Mastériens lists (Admin/SuperAdmin only). They open a modal with: upload zone → column mapping preview → validation errors list → confirm import.
9. **BibTeX and CSV export** buttons appear on every publication list (public and dashboard).
10. **The sidebar** must show a notification badge count on the bell icon and on relevant nav items (e.g., "Publications" shows the count of SOUMIS items for admin roles).
