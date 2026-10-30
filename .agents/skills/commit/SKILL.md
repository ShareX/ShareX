---
name: commit
description: Commit the current changes to the active branch with a short, plain commit message. Use when asked to commit or to save work to git.
---

# Commit

Commit the current changes to the branch that is already checked out. If text was passed to this command, use it as the basis for the message: $ARGUMENTS

## Steps

1. Run `git status`, `git diff HEAD`, and `git log -5 --oneline`.
2. If there is nothing to commit, say so and stop. If HEAD is detached or a merge or rebase is in progress, say so and stop.
3. If files are already staged, commit only those. Otherwise stage all changes by file name. Leave out secrets such as `.env` files and credentials, and tell the user about anything left out.
4. Run `git commit -m "<message>"`.
5. Report the short hash, the branch, and the message.

## Message

- One line, under 72 characters.
- Say what changed in plain words, as if telling a teammate.
- Start with a capitalized verb in the imperative: Add, Fix, Update, Remove.
- No trailing period, no type prefix such as `feat:`, no scope, no emoji.
- Name the thing that changed, not the file or the process: `Fix crash when saving empty image`, not `Update Form1.cs`.
- Add a body only when the reason is not obvious from the subject line. Keep it to one or two short sentences.

## Rules

- Do not create or switch branches.
- Do not push, amend, or rebase.
