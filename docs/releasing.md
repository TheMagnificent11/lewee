# Releasing

Releases are managed by GitHub Actions. Release workflows are available from the
repository's default branch (`develop`).

## Prerequisites

The repository must have the `RELEASE_APP_ID` and `RELEASE_APP_PRIVATE_KEY`
secrets configured for an installed GitHub App with `contents: write` and
`pull_requests: write` permissions. The App token lets release-created pull
requests trigger the normal checks.

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
