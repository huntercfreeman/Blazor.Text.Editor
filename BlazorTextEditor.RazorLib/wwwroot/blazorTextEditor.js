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
        
        if (textEditorBody.scrollTop + textEditorBody.offsetHeight > 
            textEditorBody.scrollHeight)
        {
            textEditorBody.scrollTop = textEditorBody.scrollHeight -
                textEditorBody.offsetHeight;
        }

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

        if (textEditorBody.scrollTop + textEditorBody.offsetHeight > 
            textEditorBody.scrollHeight)
        {
            textEditorBody.scrollTop = textEditorBody.scrollHeight -
                textEditorBody.offsetHeight;
        }
        
        if (textEditorBody.scrollLeft + textEditorBody.offsetWidth > 
            textEditorBody.scrollWidth)
        {
            textEditorBody.scrollLeft = textEditorBody.scrollWidth -
                textEditorBody.offsetWidth;
        }

        if (textEditorGutter) {
            textEditorGutter.scrollTop = textEditorBody.scrollTop;
        }
    },
    getElementMeasurementsInPixelsById: function (elementId) {
        let elementReference = document.getElementById(elementId);

        return this.getElementMeasurementsInPixelsByElementReference(elementReference);
    },
    getElementMeasurementsInPixelsByElementReference: function (elementReference) {
        if (!elementReference) {
            return {
                ScrollLeft: 0,
                ScrollTop: 0,
                ScrollWidth: 0,
                ScrollHeight: 0,
                Width: 0,
                Height: 0,
            };
        }

        return {
            ScrollLeft: elementReference.scrollLeft,
            ScrollTop: elementReference.scrollTop,
            ScrollWidth: elementReference.scrollWidth,
            ScrollHeight: elementReference.scrollHeight,
            Width: elementReference.offsetWidth,
            Height: elementReference.offsetHeight,
        };
    }
}