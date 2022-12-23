window.blazorTextEditor = {
    focusHtmlElementById: function (elementId) {
        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }

        element.focus();
    },
    scrollElementIntoView: function (intersectionObserverMapKey,
                                     elementId) {

        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }
        
        element.scrollIntoView({
            block: "nearest",
            inline: "nearest"
        });
    },
    preventDefaultOnWheelEvents: function (elementId) {

        let element = document.getElementById(elementId);

        if (!element) {
            return;
        }
        
        element.addEventListener('wheel', (event) => {
            event.preventDefault();
        }, {
            passive: false,
        });
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

        if (!element) {
            return {
                WidthInPixels: 0,
                HeightInPixels: 0
            }
        }
        
        return {
            WidthInPixels: element.offsetWidth,
            HeightInPixels: element.offsetHeight
        }
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let element = document.getElementById(elementId);

        if (!element) {
            return {
                RelativeX: 0,
                RelativeY: 0,
                RelativeScrollLeft: 0,
                RelativeScrollTop: 0
            }
        }

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
    mutateScrollVerticalPositionByPixels: function (textEditorBodyId, gutterElementId, pixels) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);
        
        if (!textEditorBody) {
            return;
        }
        
        textEditorBody.scrollTop += pixels;

        if (textEditorGutter) {
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    mutateScrollHorizontalPositionByPixels: function (textEditorBodyId, gutterElementId, pixels) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorBody) {
            return;
        }
        
        textEditorBody.scrollLeft += pixels;

        if (textEditorGutter) {
            textEditorGutter.scrollLeft = textEditorBody.scrollLeft;
        }
    },
    setScrollPosition: function (textEditorBodyId, gutterElementId, scrollLeft, scrollTop) {
        let textEditorBody = document.getElementById(textEditorBodyId);
        let textEditorGutter = document.getElementById(gutterElementId);

        if (!textEditorBody) {
            return;
        }
        
        if (scrollLeft || scrollLeft === 0) {
            textEditorBody.scrollLeft = scrollLeft;
        }
        
        if (scrollTop || scrollTop === 0) {
            textEditorBody.scrollTop = scrollTop;
        }

        if (textEditorGutter) {
            textEditorGutter.scrollLeft = textEditorBody.scrollLeft;
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    getScrollPosition: function (textEditorBodyId) {
        let textEditorContent = document.getElementById(textEditorBodyId);

        if (!textEditorContent) {
            return {
                ScrollLeftInPixels: 0,
                ScrollTopInPixels: 0
            };
        }

        return {
            ScrollLeftInPixels: textEditorContent.scrollLeft,
            ScrollTopInPixels: textEditorContent.scrollTop
        };
    }
}