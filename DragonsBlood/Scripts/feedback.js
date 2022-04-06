var footer = $('#feedbackModalFooter');
var body = $('#feedbackModalBody');

function onFeedbackSuccess(content) {
    body.html(content);
    footer.hide();
}

$('#FeedbackModal').on('hidden.bs.modal', function (e) {
    restoreFeedbackBody();
})
function restoreFeedbackBody() {
    var content = '<p>' +
        'Please us the box below to send us your feedback, we appreciate any comments you have regarding this website.' +
        'If you like, do not like, or simply want to see a new feature let us know.' +
        '</p>' +
        '<textarea name="feedback" id="feedback" class="form-control"></textarea>';

    body.html(content);
    footer.show();
}