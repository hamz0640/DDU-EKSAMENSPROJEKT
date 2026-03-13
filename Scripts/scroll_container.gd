extends HFlowContainer


func _on_scroll_container_resized() -> void:
	custom_minimum_size.x = get_parent().size.x - 10.0
