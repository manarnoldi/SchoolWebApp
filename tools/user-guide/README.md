# User guides

Two guides, opened from the book icons at the top of every page:

- **User Guide** (`user/`): day-to-day use, for everyone.
- **Administrator Guide** (`admin/`): setting up and running the system, for Administrators and SuperAdministrators.

The API serves them (`UserGuideController`, `GET api/userGuide/user|admin`):
- only to someone signed in;
- only while *Global Settings > Security > Show user guides* is on (switch it off on the demo site);
- without the parts for paid modules the school's licence doesn't include.

## Chapters

Each chapter is one file, `NN-name.html`, and the files are taken in name order. A chapter looks like this:

```html
<section class="chapter" id="finance" data-module="Finance">   <!-- data-module only for a paid module's chapter -->
<h1 class="ch">Finance</h1>
<p>...</p>
<h2 class="sub" id="fin-payments">Fee Payments</h2>            <!-- sections: these make the contents -->
<h3>Recording a payment</h3>
<ol class="steps"><li>...</li></ol>
<figure class="shot"><img src="assets/guide/fin-payments.png" alt="..."><figcaption>...</figcaption></figure>
<!--module:Academics--> a part that needs another paid module <!--/module:Academics-->
</section>
```

- Modules are `Academics`, `Finance`, `Payroll`, `Elections` and `Communications`.
- Every id must be unique within the guide. Prefix ids with the chapter's short name.
- CSS numbers the chapters and sections, so don't number them yourself.
- Styles you can use:
  - `<span class="ui">Save</span>` for a button or field name.
  - `<span class="path">Finance &gt; Fees &gt; Fee Payments</span>` for a menu path.
  - `.callout` boxes: plain, `.tip`, or `.important`, each with `<span class="lbl">Label.</span>`.
  - `ol.steps` for numbered steps.
  - Tables.

`user/01-getting-started.html` is the model for tone and layout.

## Screenshots

`NN-name.shots.json` next to a chapter lists that chapter's screenshots. The format is described at the top of `capture.js`.

`capture.js` takes them from a running copy of ShuleNova with the **demo data**, never a school's live site. The PNGs go to `SchoolWebApp-Client/src/assets/guide/`.

Capture with a login that holds **Administrator + Viewer**. It sees every page, and the API refuses anything a step might save. The script hides the toasts and the View-only badge.

```
npm i --prefix %TEMP%\sn-guide-tools playwright-core
set PW=%TEMP%\sn-guide-tools\node_modules\playwright-core
node tools/user-guide/capture.js --base http://127.0.0.1:5299 --user ryan --password ... [shot names]
node tools/user-guide/build.js
```

`build.js` writes `SchoolWebApp-API/Project.API/Guides/{user,admin}-guide.html`. It reports duplicate ids, unbalanced module markers and missing screenshots. Commit the chapters, the screenshots and the built guides together.
