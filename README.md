# asset-manager test

<<<<<<< HEAD
Full-stack asset management system with secure authentication, PostgreSQL persistence, paginated APIs, and interactive asset comparison dashboards.

=======
## Netlify Deployment

This app is configured for Netlify with SPA routing support.

- `netlify.toml` sets:
	- base directory: `frontend`
	- build command: `npm run build-only`
	- publish directory: `dist`
	- SPA redirect: `/* -> /index.html (200)`
- `frontend/public/_redirects` also includes `/* /index.html 200` for history-mode routes.

## Netlify Environment Variables

Set the frontend API URL in Netlify (Site settings -> Environment variables):

- `VITE_API_BASE_URL=https://<your-backend-host>/api`

The frontend reads this in `frontend/src/services/api.ts` and all API/auth requests use that shared base URL.
>>>>>>> cc1ea9b (integrating netlify and github actions for ci/cd)
