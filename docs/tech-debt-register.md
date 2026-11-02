## Technical Debt Register

Last updated: 2026-11-02 (TD-001 and TD-002 resolved)
Total items: 2 | Estimated total effort: S + S

| ID | Category | Description | Files | Effort | Impact | Priority | Status | Added | Sprint |
|----|----------|-------------|-------|--------|--------|----------|--------|-------|--------|
| TD-001 | Code Quality | Two ground enemies walking in opposite directions collide and stop: the walker treats dynamic bodies (other enemies) as non-solid, so it never turns around — from production/epics/heretic/story-004-enemies.md | Assets/Scripts/Enemies/GroundEnemy.cs, Assets/Scripts/Enemies/EnemyBase.cs | S | Med | — | Resolved 2026-11-02 (story 005: walkers turn at other enemies, confirmed in play) | 2026-11-02 | Backlog |
| TD-002 | Test | Ledge turning was never exercised: the EnemyTest room has only a flat floor and walls — from production/epics/heretic/story-004-enemies.md | Assets/Scripts/Enemies/GroundEnemy.cs, Assets/Scripts/Enemies/EnemyTestBootstrap.cs | S | Low | — | Resolved 2026-11-02 (story 005: Level01 has a raised platform with edges, confirmed in play) | 2026-11-02 | Backlog |
