AFRAME.registerComponent('networked-hand-tracking', {
    dependencies: ['hand-tracking-controls'],

    schema: {
        options: { default: 'template:#hand-joint-template; attachTemplateToLocal:false' }
    },

    init: function () {
        var el = this.el;
        this.trackingControls = el.components['hand-tracking-controls'];
        this.trackingControls.initDotsModel = this.initDotsModel();

        this.onExitVR = AFRAME.utils.bind(this.onExitVR, this);
        el.sceneEl.addEventListener('exit-vr', this.onExitVR);
    },

    initDotsModel: function () {
        this.trackingControls.initDotsModel();

        var jointEls = this.trackingControls.jointEls;

        for (var i = 0; i < jointEls.length; i++) {
            jointEls[i].setAttribute('networked', this.data.options);
        }
    },

    onExitVR: function () {
        var jointEls = this.trackingControls.jointEls;
        this.trackingControls.jointEls = [];

        for (var i = 0; i < jointEls.length; i++) {
            jointEls[i].removeAttribute('networked');
        }
    },
});