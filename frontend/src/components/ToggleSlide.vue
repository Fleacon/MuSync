<script setup>
const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['update:modelValue'])
</script>

<template>
  <label class="switch">
    <input
      type="checkbox"
      :checked="modelValue"
      @change="emit('update:modelValue', $event.target.checked)"
    />
    <span class="slider round"></span>
  </label>
</template>

<style scoped>
.switch {
  --switch-width: 3.5rem;
  --switch-height: 1.9rem;
  --knob-size: 1.45rem;
  --knob-padding: 0.15rem;

  position: relative;
  display: inline-block;
  width: var(--switch-width);
  height: var(--switch-height);
}

.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #1d546c;
  transition: 0.2s;
  border: 0.1rem solid #102f3c;
}

.slider:before {
  position: absolute;
  content: '';
  height: var(--knob-size);
  width: var(--knob-size);
  left: var(--knob-padding);
  bottom: var(--knob-padding);
  background-color: white;
  transition: 0.2s;
}

input:checked + .slider {
  background-color: #4452bb;
}

input:focus + .slider {
  box-shadow: 0 0 1px #4452bb;
}

input:checked + .slider:before {
  transform: translateX(calc(var(--switch-width) - var(--knob-size) - var(--knob-padding) * 3));
}

.slider.round {
  border-radius: var(--switch-height);
}

.slider.round:before {
  border-radius: 50%;
}
</style>
