<script setup lang="ts">
import { CheckIcon, ChevronsUpDownIcon } from 'lucide-vue-next'
import { computed, ref } from 'vue'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from '@/components/ui/command'
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from '@/components/ui/popover'

export interface ComboboxOption {
  value: string | number
  label: string
  disabled?: boolean
}

interface ComboboxProps {
  modelValue?: string
  options?: ComboboxOption[]
  placeholder?: string
  searchPlaceholder?: string
  emptyMessage?: string
  disabled?: boolean
  class?: string
}

const props = withDefaults(defineProps<ComboboxProps>(), {
  modelValue: '',
  options: () => [],
  placeholder: 'Select option...',
  searchPlaceholder: 'Search...',
  emptyMessage: 'No option found.',
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const open = ref(false)

const selectedOption = computed(() =>
  props.options.find(option => option.value === props.modelValue),
)

function selectOption(selectedValue: string) {
  const newValue = selectedValue === props.modelValue ? '' : selectedValue
  emit('update:modelValue', newValue)
  open.value = false
}
</script>

<template>
  <Popover v-model:open="open">
    <PopoverTrigger as-child>
      <Button variant="outline" role="combobox" :aria-expanded="open" :disabled="disabled"
        :class="cn('w-full justify-between', props.class)">
        {{ selectedOption?.label || placeholder }}
        <ChevronsUpDownIcon class="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-full p-0">
      <Command>
        <CommandInput class="h-9" :placeholder="searchPlaceholder" />
        <CommandList>
          <CommandEmpty>{{ emptyMessage }}</CommandEmpty>
          <CommandGroup>
            <CommandItem v-for="option in options" :key="option.value" :value="option.value" :disabled="option.disabled"
              @select="(ev) => {
                selectOption(ev.detail.value as string)
              }">
              {{ option.label }}
              <CheckIcon :class="cn(
                'ml-auto h-4 w-4',
                modelValue === option.value ? 'opacity-100' : 'opacity-0',
              )" />
            </CommandItem>
          </CommandGroup>
        </CommandList>
      </Command>
    </PopoverContent>
  </Popover>
</template>
