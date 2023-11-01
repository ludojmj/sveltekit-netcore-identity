# All-In-One CRUD

```bash
git clone https://github.com/ludojmj/sveltekit-netcore-identity.git
```

The aim of this project is to gather, in a single place, useful front and back ends development tools:

- A Database with SQLite;
- A Web API server with .NET Core 6.x;
- A Svelte/SvelteKit client App;
- A link to an external service for identity management, authorization, and API security.

## Quick start (Development)

Server

```bash
cd <myfolder>/Server
dotnet run
```

Client

```bash
cd <myfolder>/client
npm install
npm run dev
```

---

## Inspiration

- SQLite database powered by: <https://www.sqlite.org>
- Server based on API mechanisms of: <https://reqres.in/api/whatever>
- Svelte template client borrowed from: <https://kit.svelte.dev>
- Identity service powered by: <https://demo.duendesoftware.com>
- Identity client borrowed from: <https://github.com/AxaFrance/oidc-client>
- CSS borrowed from: <https://getbootstrap.com>
- SVG borrowed from: <https://creativecommons.org>

---

## Manufacturing process steps

### >>>>> SQLite database

#### Overwrite database if needed

```bash
cd <myfolder>
sqlite3 Server/App_Data/stuff.db < Server/App_Data/create_tables.sql
```

### >>>>> .NET Core 6.x Web API server

#### Create the server project

```bash
cd <myfolder>
dotnet new gitignore
dotnet new webapi -n Server
dotnet new xunit -n Server.UnitTest
```

#### Generate the model from the database for the Web API server

```bash
dotnet tool install --global dotnet-ef
cd <myfolder>/Server
dotnet ef dbcontext scaffold "Data Source=App_Data/stuff.db" Microsoft.EntityFrameworkCore.Sqlite \
--output-dir DbModels --context-dir DbModels --context StuffDbContext --force
```

#### Run the tests

```bash
cd <myfolder>/Server.UnitTest
dotnet restore
dotnet build
dotnet test
dotnet test /p:CollectCoverage=true
```

#### Run the Web API server

```bash
cd <myfolder>/Server
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### >>>>> Svelte client App

#### Create the client project

```bash
cd <myfolder>
npm create vite@latest client -- --template svelte
```

#### Run the client App

```bash
cd <myfolder>/client
npm install
npm run dev
```

#### Client App settings

##### Development mode (```<myfolder>/client/.env.development```)

- Client consuming an embedded mocked API (http port 5173)

```bash
VITE_API_URL="http://localhost:5173/mock/stuff"
```

- Client (http port 5173) consuming a distant API (https port 5001)

```bash
VITE_API_URL="https://localhost:5001/api/stuff"
```

- Client (https port 5001) and Server (https port 5001) together in the same Web app

==> Files inside "<myfolder>/client/build" directory must be copied into "<myfolder>/Server/wwwroot" directory

==> See: buildall.sh

```bash
VITE_API_URL="https://localhost:5001/api/stuff"
```

##### Production mode (```<myfolder>/client/.env.production```)

- Client and Server together in the same Web app hosted on a real domain

```bash
VITE_API_URL="https://www.example.com/api/stuff"
```

==> Files inside "<myfolder>/client/build" directory must be copied into "<myfolder>/Server/wwwroot" directory

==> See: buildall.sh

---

## Troubleshooting

### _Unable to register/update a service worker for scope 'http://localhost:5173/': Storage access is restricted in this context due to user settings or incognito mode._

**When?**

- When using Firefox;
- Running http://localhost:5173 (actually using http instead of https);
- The website is stucked, you must open the debug console to see this message.

**Why?**

- Firefox blocks third-party cookies between http and https.

**How to solve?**

- Allow your website (http://localhost:5173) to use cookies and site data;
- Click to see how to proceed: https://support.mozilla.org/en-US/kb/block-websites-storing-cookies-site-data-firefox

### _An error occured. Please try again later._

**When?**

- Creating a record in the SQLite database _stuff.db_ running Linux on Azure;
- The "real" error (not displayed in Production) is: _SQLite Error 5: 'database is locked'_;
- There is a restricted write access to the file on Linux web app when running on Azure.

**How to solve?**

- ==> Either use a real database or deploy the web app on Azure choosing Windows OS.

### _SQLite Error 1: 'no such table: t_stuff'_

**When?**

- Running the Svelte client App (```npm run dev```);
- Connecting to: <http://localhost:5173/>.

**How to solve?**

- ==> Create the database _stuff.db_ (```sqlite3 Server/App_Data/stuff.db < Server/App_Data/create_tables.sql```).

### _Network Error_

**When?**

- Running the Svelte client App (```npm run dev```);
- Connecting to: <http://localhost:5173/>.

**How to solve?**

- ==> Start the .NET server (```dotnet run```) before the Svelte client App (```npm run dev```).

### _Your connection is not private_ (NET::ERR_CERT_AUTHORITY_INVALID)

**When?**

- Running the .NET Core server (dotnet run);
- Connecting to: <https://localhost:5001/health>.

**How to solve?**

> Firefox

- ==> Click `Advanced settings` button;
- ==> Click on the link to continue to the assumed unsafe localhost site;
- ==> Accept self-signed localhost certificate.

> Chrome/Chromium

- ==> In the browser, click "Not secure" in front of the uri <https://localhost:5001/health>;
- ==> Click "Certificate is not valid";
- ==> Click "Details" tab;
- ==> Export the certificate as (for instance) "myLocalhostCert.crt";
- ==> Then run in a terminal (the first command line may be useless if you've already got the `certutil` command):

```bash
sudo apt install libnss3-tools
```

```bash
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n <nickname> -i <export_path>/myLocalhostCert.crt
```

- ==> Close and reopen the browser;
- ==> You can see your registered certificate (_org-localhost_) in: `Settings` / `Privacy and security` / `Security` / `Manage device certificates` / `Server` tab.

> Chrome/Chromium - alternative fix

If you don't want to register the certificate, you can run Chrome/Chromium with `--ignore-certificate-errors` command line option:

```bash
chromium --ignore-certificate-errors
```

N.B. Replace "`chromium`" run command by your (depending on your distribution) actual Chrome/Chromium launcher: `chromium-browser`, `chrome`, `chrome-browser` or `google-chrome`, ...

### _You do not have permission to view this directory or page._

**When?**

- Browsing the web site on a Azure Windows instance.

**How to solve?**

- ==> Add the web.config file since you've got IIS running;
- ==> On Linux, the web.config file is useless
(Update your http headers according to the suitable Web Server configuration file).
