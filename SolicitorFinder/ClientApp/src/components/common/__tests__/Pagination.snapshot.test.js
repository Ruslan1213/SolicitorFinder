import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import Pagination from '../Pagination.vue'

describe('Pagination Snapshots', () => {
  it('matches snapshot on first page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 1,
        totalPages: 10,
        totalCount: 200,
        maxVisible: 5
      }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('matches snapshot on middle page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 5,
        totalPages: 10,
        totalCount: 200,
        maxVisible: 5
      }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('matches snapshot on last page', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 10,
        totalPages: 10,
        totalCount: 200,
        maxVisible: 5
      }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('matches snapshot with few pages', () => {
    const wrapper = mount(Pagination, {
      props: {
        currentPage: 2,
        totalPages: 3,
        totalCount: 60,
        maxVisible: 5
      }
    })

    expect(wrapper.html()).toMatchSnapshot()
  })
})
