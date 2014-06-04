cd %1
if defined git (
"%git%" rev-parse HEAD > %2
) else (
type nul > %2
)