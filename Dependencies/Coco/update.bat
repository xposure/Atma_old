@FOR /D /r %%G in (..\..\Source\Core\Atma.Framework\Parsing\*) DO (
@echo Processing %%G\Grammar.atg 
@coco %%G\Grammar.atg
@del %%G\*.old
)
