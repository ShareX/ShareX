# Model team

This repository's Claude Code project settings (`.claude/settings.json`) configure Fable as the advisor tool and Haiku as the default subagent model. `.claude/agents/` adds a Haiku `researcher` subagent and an Opus `reviewer` subagent. The main model is whatever the session was started with (often Sonnet or Opus); it is not set here.

These rules are for Claude Code only. Rules for every coding agent live in `AGENTS.md`.

## Main model: builds
- Writes all the code and makes all the edits. Owns the conversation with the user.
- Does not hand implementation to Haiku subagents; their output is research, not code.

## Advisor (Fable): decides, never writes code
Consult the advisor, when the advisor tool is available, at these points, and do not skip them on non-trivial tasks:
1. Before committing to a plan or approach for anything beyond a small, obvious edit, such as adding or changing a platform service or touching more than one platform project.
2. When the same error or failing test comes back after two fix attempts.
3. Before declaring a multi-step task done, committed or pushed.

Do not consult it for routine steps (reading files, simple edits, running commands). Each call re-reads the whole transcript at the advisor's rates, so keep to these points. When its advice conflicts with evidence you have (a file's contents, a command's result), say so instead of following it blindly.

## Haiku (subagents): searches and researches
- For broad codebase searches (for example, finding every caller of a platform API across the shared projects), documentation lookups and web research, use the `researcher` subagent, which is pinned to Haiku. Launch independent ones in parallel.
- If you use another agent type for research instead (general-purpose, Explore, Plan), pass `model: "haiku"` when spawning it. The `CLAUDE_CODE_SUBAGENT_MODEL` default in the project settings applies only to local sessions: cloud sessions whose provider is managed by the host, such as Claude Code on the web, ignore it from project settings, and it never changes the built-in Explore and Plan agents.
- Give each a narrow question and ask for a short answer with file paths or URLs, so raw output stays out of the main context.
- A single known file or symbol does not need a subagent; read or grep it directly.

## Opus (`reviewer` subagent): checks before it ships
- After a non-trivial code change, and before a commit or push, run the `reviewer` subagent on the diff.
- Fix what it confirms. Re-run it only when the fixes were substantial.
- Skip it for one-line, docs-only or configuration-only changes.
