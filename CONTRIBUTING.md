# How to contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## Filing issues
- Don't be afraid to ask any question about the project, including suggestions to change how things currently are.
- Keep the conversation polite and respectful. This way, all parties will take an interest in your question and will be more proactive into helping.
- For bugs, the best way to get your bug fixed is to be as detailed as you can be about the problem. Providing a minimal project with steps to reproduce the problem is ideal. Even though this might be painful, it will speed up the resolution to the problem.

GitHub supports [markdown](https://help.github.com/articles/github-flavored-markdown/), so when filing bugs make sure you check the formatting before clicking submit.

## Contributing code and content

**Identifying the scale**

If you would like to contribute to one of our repositories, first identify the scale of what you would like to contribute. If it is small (grammar/spelling or a bug fix) feel free to start working on a fix. If you are submitting a feature or substantial code contribution, please discuss it with the team.

You can also read these two blogs posts on contributing code: [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html) by Miguel de Icaza and [Don't "Push" Your Pull Requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/) by Ilya Grigorik. Note that all code submissions will be reviewed and tested by team members. Of course (where appropriate), tests will be required.

**Obtaining the source code**

If you are an outside contributer, please fork the appropriate repository you would like to contribute to. See the GitHub documentation on [forking a repo](https://help.github.com/articles/fork-a-repo/) if you have any questions about this.

**Building our Repositories**

All repositories are designed to be simple to get started. Once you have pulled down your own fork, just open up the solution file (`<something>.sln` for .NET) and then compile the solution.

**Submitting a Pull Request**

If you don't know what a pull request is read this article: https://help.github.com/articles/using-pull-requests. Make sure the repository can build and all tests pass. Familiarize yourself with the project workflow and our coding conventions.

Pull requests should all be done to the `master` branch.

When a pull request is accepted, squashed and merged (by the repo Administrators), we then manually create a Git tag, which will then be the trigger point to creating a new NuGet package. As such, we don't always create new NuGet packages on every Pull Request ... which is why this is a manual step.

**Commit/Pull Request Format**

```
Summary of the changes (Less than 80 chars)
 - Detail 1
 - Detail 2

Addresses #bugnumber (in this specific format)
```

**Tests**

-  Tests need to be provided for every bug/feature that is completed.
-  Tests only need to be present for issues that need to be verified by QA (e.g. not tasks)
-  If there is a scenario that is far too hard to test there does not need to be a test for it.
  - "Too hard" is determined by the team as a whole.

---