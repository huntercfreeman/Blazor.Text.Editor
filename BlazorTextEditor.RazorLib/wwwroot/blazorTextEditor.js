window.blazorTextEditor = {
    cursorIntersectionObserverMap: new Map(),
    initializeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey,
                                                              scrollableParentElementId,
                                                              cursorElementId) {

        let scrollableParent = document.getElementById(scrollableParentElementId);

        let options = {
            root: scrollableParent,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let intersectionObserverMapValue = this.cursorIntersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
                    .find(x => x.CursorElementId === entry.target.id);

                cursorTuple.IsIntersecting = entry.isIntersecting;
            }
        }, options);

        let cursorIsIntersectingTuples = [];

        let cursorElement = document.getElementById(cursorElementId);
        
        intersectionObserver.observe(cursorElement);

        cursorIsIntersectingTuples.push({
            CursorElementId: cursorElementId,
            IsIntersecting: false
        });

        this.cursorIntersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            CursorIsIntersectingTuples: cursorIsIntersectingTuples
        });
    },
    revealCursor: function (intersectionObserverMapKey,
                            cursorElementId) {

        let intersectionObserverMapValue = this.cursorIntersectionObserverMap
            .get(intersectionObserverMapKey);

        let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
            .find(x => x.CursorElementId === cursorElementId);

        if (!cursorTuple.IsIntersecting) {
            let cursorElement = document.getElementById(cursorElementId);

            cursorElement.scrollIntoView({
                block: "nearest",
                inline: "nearest"
            });
        }
    },
    disposeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey) {

        let intersectionObserverMapValue = this.cursorIntersectionObserverMap
            .get(intersectionObserverMapKey);
        
        let intersectionObserver = intersectionObserverMapValue.IntersectionObserver;

        this.cursorIntersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserver.disconnect();
    },
    measureCharacterWidthAndRowHeight: function (elementId, amountOfCharactersRendered) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                CharacterWidthInPixels: 5,
                RowHeightInPixels: 5
            }
        }
        
        let fontWidth = element.offsetWidth / amountOfCharactersRendered;

        return {
            CharacterWidthInPixels: fontWidth,
            RowHeightInPixels: element.offsetHeight
        }
    },
    measureWidthAndHeightOfTextEditor: function (elementId) {
        let element = document.getElementById(elementId);

        return {
            WidthInPixels: element.offsetWidth,
            HeightInPixels: element.offsetHeight
        }
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let element = document.getElementById(elementId);

        let bounds = element.getBoundingClientRect();

        let x = clientX - bounds.left;
        let y = clientY - bounds.top;

        return {
            RelativeX: x,
            RelativeY: y,
            RelativeScrollLeft: element.scrollLeft,
            RelativeScrollTop: element.scrollTop
        }
    },
    virtualizationIntersectionObserverMap: new Map(),
    initializeVirtualizationIntersectionObserver: function (intersectionObserverMapKey,
                                              virtualizationDisplayDotNetObjectReference,
                                              scrollableParentFinder,
                                              boundaryIds) {

        let scrollableParent = scrollableParentFinder.parentElement;

        scrollableParent.addEventListener("scroll", (event) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < intersectionObserverMapValue.BoundaryIdIsIntersectingTuples.length; i++) {
                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIsIntersectingTuples[i];

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync("OnScrollEventAsync", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, true);

        let options = {
            root: scrollableParent,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
                .get(intersectionObserverMapKey);

            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIsIntersectingTuples
                    .find(x => x.BoundaryId === entry.target.id);

                boundaryTuple.IsIntersecting = entry.isIntersecting;

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync("OnScrollEventAsync", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, options);

        let boundaryIdIsIntersectingTuples = [];

        for (let i = 0; i < boundaryIds.length; i++) {

            let boundaryElement = document.getElementById(boundaryIds[i]);

            intersectionObserver.observe(boundaryElement);

            boundaryIdIsIntersectingTuples.push({
                BoundaryId: boundaryIds[i],
                IsIntersecting: false
            });
        }

        this.virtualizationIntersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            BoundaryIdIsIntersectingTuples: boundaryIdIsIntersectingTuples
        });

        virtualizationDisplayDotNetObjectReference
            .invokeMethodAsync("OnScrollEventAsync", {
                ScrollLeftInPixels: scrollableParent.scrollLeft,
                ScrollTopInPixels: scrollableParent.scrollTop
            });
    },
    disposeVirtualizationIntersectionObserver: function (intersectionObserverMapKey) {
        
        let intersectionObserverMapValue = this.virtualizationIntersectionObserverMap
            .get(intersectionObserverMapKey);

        let intersectionObserver = intersectionObserverMapValue.IntersectionObserver;

        this.virtualizationIntersectionObserverMap.delete(intersectionObserverMapKey);

        intersectionObserver.disconnect();
    },
    readClipboard: async function () {
        // First, ask the Permissions API if we have some kind of access to
        // the "clipboard-read" feature.

        try {
            return await navigator.permissions.query({name: "clipboard-read"}).then(async (result) => {
                // If permission to read the clipboard is granted or if the user will
                // be prompted to allow it, we proceed.

                if (result.state === "granted" || result.state === "prompt") {
                    return await navigator.clipboard.readText().then((data) => {
                        return data;
                    });
                } else {
                    return "";
                }
            });
        } catch (e) {
            return "";
        }
    },
    setClipboard: function (value) {
        // Copies a string to the clipboard. Must be called from within an
        // event handler such as click. May return false if it failed, but
        // this is not always possible. Browser support for Chrome 43+,
        // Firefox 42+, Safari 10+, Edge and Internet Explorer 10+.
        // Internet Explorer: The clipboard feature may be disabled by
        // an administrator. By default a prompt is shown the first
        // time the clipboard is used (per session).
        if (window.clipboardData && window.clipboardData.setData) {
            // Internet Explorer-specific code path to prevent textarea being shown while dialog is visible.
            return window.clipboardData.setData("Text", text);

        } else if (document.queryCommandSupported && document.queryCommandSupported("copy")) {
            var textarea = document.createElement("textarea");
            textarea.textContent = value;
            textarea.style.position = "fixed";  // Prevent scrolling to bottom of page in Microsoft Edge.
            document.body.appendChild(textarea);
            textarea.select();
            try {
                return document.execCommand("copy");  // Security exception may be thrown by some browsers.
            } catch (ex) {
                console.warn("Copy to clipboard failed.", ex);
                return false;
            } finally {
                document.body.removeChild(textarea);
            }
        }
    },
    getContextMenuFixedPositionRelativeToElement: function (elementId) {
        let element = document.getElementById(elementId);

        let bounds = element.getBoundingClientRect();

        return {
            OccurredDueToMouseEvent: false,
            LeftPositionInPixels: bounds.left,
            TopPositionInPixels: bounds.top + bounds.height
        }
    },
    localStorageSetItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    localStorageGetItem: function (key, value) {
        return localStorage.getItem(key);
    },
}

Blazor.registerCustomEventType('keydownwithpreventscroll', {
    browserEventName: 'keydown',
    createEventArgs: e => {
        
        let preventDefaultOnTheseKeys = [
            "ArrowLeft",
            "ArrowDown",
            "ArrowUp",
            "ArrowRight",
            "Home",
            "End",
            "Space",
            "Enter",
        ];
        
        let preventDefaultOnTheseCodes = [
            "Space",
            "Enter",
        ];
        
        if (preventDefaultOnTheseKeys.indexOf(e.key) !== -1 ||
            preventDefaultOnTheseCodes.indexOf(e.code) !== -1) {
            e.preventDefault();
        }

        return {
            Type: e.type,
            MetaKey: e.metaKey,
            AltKey: e.altKey,
            ShiftKey: e.shiftKey,
            CtrlKey: e.ctrlKey,
            Repeat: e.repeat,
            Location: e.location,
            Code: e.code,
            Key: e.key
        };
    }
});