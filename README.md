# Cleaner Job Details - [Andrew Borondia](https://aborondia.netlify.app/projects#IXr0QXCawa)

I created this to replace a paper based job reporting system used by my wife's work so she wouldn't have to travel to the office as much. I set up a backend which includes the entire flow for registering/saving users, authentication, saving & emailing (via [Sendgrid](https://sendgrid.com/)) reports, etc. The reports are compiled into a single pdf via a custom implementation of [sharpPDF](https://sharppdf.sourceforge.net/) that converts the pdf into a filestream & attaches it to the emails without the need to save it on the users system (to avoid issues with WebGL & make the emailing process simpler for the end user). The frontend is a **Unity** app built using the **UI Toolkit**. I decided to go for a **WebGL** build to simplify getting started for new users, and to make pushing updates very easy when needed. Yes, I love my wife.

![A screenshot of the job report creation screen](https://i.postimg.cc/ZKryJdWY/job-details.png)

## Security Note

This project uses Parse Server (hosted on Back4App) with the following security measures:

- **API keys visible in this repository have been rotated** and are no longer valid.
- All sensitive credentials (SendGrid key, etc.) are stored as server-side environment variables only.
- Class-Level Permissions (CLPs) on the Parse Server prevent unauthorized data modification even with the JavaScript Key.
- No `.env` files or credential stores are included in version history.
