## ## Makefile
##
##  usage:

help: # show help
	@echo ""
	@grep "^##" $(MAKEFILE_LIST) | grep -v grep
	@echo ""
	@grep "^[0-9a-zA-Z\-]*: #" $(MAKEFILE_LIST) | grep -v grep
	@echo ""

toggletest: # make single unittest or all unittest, e.g make toggletest test=TestObservableTriggers. After that make togglealltest to revert
	cd Assets/Tests/NUnitLite && (find . -iname "*.cs" | awk '{ print substr($$0, 0, length($$0)-3); }' | xargs -n 1 -I {} mv {}.cs {}.cs_) && mv $(test).cs_ $(test).cs

togglealltest: # make togglealltest
	cd Assets/Tests/NUnitLite && (find . -iname "*.cs_" | awk '{ print substr($$0, 0, length($$0)-4); }' | xargs -n 1 -I {} mv {}.cs_ {}.cs)
