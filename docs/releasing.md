# Releasing

Releases are managed by GitHub Actions. Release workflows are available from the
repository's default branch (`develop`).

## Prerequisites

### Why a GitHub App is needed

The release workflows need a credential that can push branches and tags and
open pull requests. With `GITHUB_TOKEN`, pull request workflows for `opened`,
`synchronize`, and `reopened` require manual approval before they run; most
other token-generated events do not trigger workflows. The App token allows
required checks to run automatically on release pull requests.

A GitHub App is used here purely as a machine identity for the release
workflows. It is **not** used for signing users in to a website. The workflows
exchange the App's ID and private key for a short-lived installation token
using
[`actions/create-github-app-token`](https://github.com/actions/create-github-app-token).

| Workflow | What it does with the App token |
| -------- | ------------------------------- |
| `release-start.yml` (**Release - Start**) | Checks out `develop`, generates release notes, pushes the `release/<version>` branch, and opens the pull request to `main` |
| `release-finish.yml` (**Release - Finish**) | Pushes the annotated version tag, opens the `main` to `develop` back-merge pull request, and deletes the release branch |

The **Continuous Deployment** workflow (`cd.yml`) does not use the App. It
publishes to NuGet.org with NuGet trusted publishing (OIDC) and creates the
GitHub Release with the workflow's own `github.token`.

### Create the GitHub App

Use the repository owner's account (for `TheMagnificent11/lewee`, the
`TheMagnificent11` personal account). If the repository belongs to an
organization, use the organization's settings instead.

1. In GitHub, go to **Settings** > **Developer settings** > **GitHub Apps** and
   select **New GitHub App**. For an organization, go to the organization's
   **Settings** > **Developer settings** > **GitHub Apps**.
2. Fill out the registration form:

   | Field | Value |
   | ----- | ----- |
   | **GitHub App name** | A globally unique name, such as `lewee-release-bot`. Pull requests opened by the workflows are attributed to the App's bot account; pushing existing commits does not change their authorship |
   | **Description** | Optional, such as `Creates release branches, tags, and pull requests for Lewee` |
   | **Homepage URL** | Required but not used by the workflows. Use the repository URL, `https://github.com/TheMagnificent11/lewee` |
   | **Identifying and authorizing users** (Callback URL) | Leave empty. Clear **Expire user authorization tokens** and **Request user authorization (OAuth) during installation**. The App never signs in users |
   | **Post installation** (Setup URL) | Leave empty |
   | **Webhook** > **Active** | Clear this checkbox. The workflows call GitHub directly, so no webhook URL or secret is needed |
   | **Repository permissions** > **Contents** | **Read and write** |
   | **Repository permissions** > **Pull requests** | **Read and write** |
   | **Repository permissions** > **Metadata** | **Read-only** (selected automatically) |
   | Other repository, organization, and account permissions | Leave as **No access** |
   | **Subscribe to events** | Leave everything unchecked |
   | **Where can this GitHub App be installed?** | **Only on this account** |

3. Select **Create GitHub App**.

### Collect the App ID and private key

1. On the App's settings page (**Settings** > **Developer settings** >
   **GitHub Apps** > **Edit** next to the App), copy the **App ID** shown under
   **About**. Do not use the Client ID.
2. Scroll to **Private keys** and select **Generate a private key**. GitHub
   downloads a `.pem` file. Keep it secret and never commit it. See
   [managing private keys](https://docs.github.com/en/apps/creating-github-apps/authenticating-with-a-github-app/managing-private-keys-for-github-apps)
   for details.

### Install the App on the repository

1. On the App's settings page, select **Install App** in the sidebar.
2. Select **Install** next to the account that owns the repository.
3. Choose **Only select repositories**, select `lewee`, and select **Install**.

The workflows fail to create a token if the App is not installed on the
repository.

### Add the repository secrets

1. In the repository, go to **Settings** > **Secrets and variables** >
   **Actions**.
2. Select **New repository secret** and create `RELEASE_APP_ID` with the App ID
   as its value.
3. Create `RELEASE_APP_PRIVATE_KEY` with the **entire contents** of the
   downloaded `.pem` file as its value, including the
   `-----BEGIN RSA PRIVATE KEY-----` and `-----END RSA PRIVATE KEY-----` lines.
4. Delete the downloaded `.pem` file once the secret is saved.

### Verify the setup

Run **Release - Start** as described below. If the **Create GitHub App token**
step succeeds and the release pull request is created, the App is configured
correctly. Checks should run on that pull request because it was opened with the
App's token.

Release pull requests to `main` must be merged with a merge commit. Do not squash
or rebase them; the release workflow verifies the merge commit and Git Flow
ancestry depends on it.

## Start a release

1. In GitHub, open **Actions** and run **Release - Start** from `develop`.
2. Enter a SemVer version greater than the latest SemVer tag, such as `1.2.3`
   or `1.2.3-beta.1`. The version must not already have a tag or
   `release/<version>` branch.
3. Review the generated pull request to `main` and edit its body to finalize
   the release notes. The body becomes the annotated tag message and GitHub
   Release notes.
4. After the required checks pass, merge the pull request using a merge commit.

## Verify the release

After the merge, **Release - Finish** creates the annotated version tag on the
merge commit, opens a back-merge pull request from `main` to `develop`, and
deletes the release branch. Merge the back-merge pull request with a merge
commit.

Successful CI on `main` starts **Continuous Deployment**. It waits up to three
minutes for the tag, then packs and publishes the NuGet packages and creates a
GitHub Release with the package and symbol-package assets. A version with a
SemVer prerelease suffix is marked as a pre-release. Verify the finish and CD
workflow runs in **Actions**, confirm the tag points to the release merge
commit, and check the packages on NuGet.org and the published GitHub Release.

## Recover from failures

- **Release - Start fails:** Check that the input is valid SemVer, is greater
  than the latest tag, and has no existing tag or release branch. If the
  workflow cannot create its App token, check the repository secrets and App
  installation permissions.
- **Release - Finish fails:** Review its run logs and rerun the failed workflow.
  The finish steps tolerate an existing tag only when it is an annotated tag on
  the same merge commit, and they avoid opening a duplicate back-merge pull
  request.
- **CD reports that no tag was found:** Confirm **Release - Finish** succeeded
  and that the tag points to the `main` commit validated by CI. Once the tag is
  present, rerun the successful CI workflow on `main` to trigger CD again.
- **Publishing or GitHub Release creation fails:** Review the CD run logs and
  rerun the failed CD workflow. Package publishing skips duplicates, and an
  existing draft GitHub Release can resume asset upload and publication.
